using System;
using System.Linq;
using System.Collections.Generic;
using LearningStarter.Common;
using LearningStarter.Entities;
using Microsoft.AspNetCore.Mvc;
using LearningStarter.Data;

namespace LearningStarter.Controllers;

[ApiController]
[Route("api/student")]

public class StudentController : ControllerBase
{
    private readonly DataContext _dataContext;
    
    public StudentController(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    [HttpGet]
    public IActionResult GetAll()
    {
        var response = new Response();
        
        var data = _dataContext
            .Set<Student>()
            .Select(student => new StudentGetDto
            {
                Id = student.Id,
                Name = student.Name,
                StudentEmail = student.StudentEmail,
                Classrooms = student.Classrooms
            })
            .ToList();
        
        response.Data = data;
        return Ok(response);
    }
    
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var response = new Response();
                
        var data = _dataContext
            .Set<Student>()
            .Select(student => new StudentGetDto
            {
                Id = student.Id,
                Name = student.Name,
                StudentEmail = student.StudentEmail,
                Classrooms = student.Classrooms
            })
            .FirstOrDefault(student => student.Id == id);
        
        response.Data = data;
        return Ok(response);
    }
    
    [HttpPost]
    public IActionResult Create([FromBody] StudentCreateDto createDto)
    {
        var response = new Response();
        
        if(string.IsNullOrEmpty(createDto.Name))
        {
             response.AddError(nameof(createDto.Name), "Student Name must not be empty.");   
        }
        
        if(response.HasErrors)
        {
            return BadRequest(response);
        }
        var studentToCreate = new Student
        {
            Name = createDto.Name,
            StudentEmail = createDto.StudentEmail
        };
        
        _dataContext.Set<Student>().Add(studentToCreate);
        _dataContext.SaveChanges();
        
        var studentToReturn  = new StudentGetDto
        {
            Id = studentToCreate.Id,
            Name = studentToCreate.Name,
            StudentEmail = studentToCreate.StudentEmail
        };
        
        response.Data = studentToReturn;
        return Created("", response);
    }
    
    [HttpPut("{id}")]
    public IActionResult Update([FromBody] StudentUpdateDto updateDto, int id)
    {
        var response = new Response();
        
        if(string.IsNullOrEmpty(updateDto.Name))
        {
             response.AddError(nameof(updateDto.Name), "Student Name must not be empty.");   
        }
        if(string.IsNullOrEmpty(updateDto.StudentEmail))
        {
             response.AddError(nameof(updateDto.StudentEmail), "Student Email must not be empty.");   
        }
        
        var studentToUpdate = _dataContext.Set<Student>()
            .FirstOrDefault(student => student.Id == id);
        
        if(studentToUpdate == null)
        {
            response.AddError("id", "Student not found.");
        }
        
        if(response.HasErrors)
        {
            return BadRequest(response);
        }
        
        studentToUpdate.Name = updateDto.Name;
        studentToUpdate.StudentEmail = updateDto.StudentEmail;
        
        if(response.HasErrors)
        {
            return BadRequest(response);
        }
        
        _dataContext.SaveChanges();
        
        var studentToReturn = new StudentGetDto
        {
            Id = studentToUpdate.Id,
            Name = studentToUpdate.Name,
            StudentEmail = studentToUpdate.StudentEmail
        };
        
        response.Data = studentToReturn;
        
        return Ok(response);
    }
    
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var response = new Response();
        
        var studentToDelete = _dataContext.Set<Student>()
            .FirstOrDefault(student => student.Id == id);
        
        if(studentToDelete == null)
        {
            response.AddError("id", "Student not found.");
        }
        
        if(response.HasErrors)
        {
            return BadRequest(response);
        }
                    
        _dataContext.Set<Student>().Remove(studentToDelete);
        _dataContext.SaveChanges();
        
        response.Data = true;
        return Ok(response);
    }
}