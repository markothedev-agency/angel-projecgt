using System;
using System.Linq;
using System.Collections.Generic;
using LearningStarter.Common;
using LearningStarter.Entities;
using Microsoft.AspNetCore.Mvc;
using LearningStarter.Data;

namespace LearningStarter.Controllers;

[ApiController]
[Route("api/classrooms")]

public class ClassroomController : ControllerBase
{
    private readonly DataContext _dataContext;
    
    public ClassroomController(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    [HttpGet]
    public IActionResult GetAll()
    {
        var response = new Response();
        
        var data = _dataContext
            .Set<Classroom>()
            .Select(classroom => new ClassroomGetDto
            {
                Id = classroom.Id,
                Name = classroom.Name,
                Description = classroom.Description,
                Channels = classroom.Channels.Select(x => new ClassroomChannelsGetDto
                {
                    Id = x.Channel.Id,
                    Name = x.Channel.Name,
                    Description = x.Channel.Description
                }).ToList(),
                Students = classroom.Students.Select(x => new ClassroomStudentsGetDto
                {
                    Id = x.Student.Id,
                    Name = x.Student.Name,
                    StudentEmail = x.Student.StudentEmail
                }).ToList()
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
            .Set<Classroom>()
            .Select(classroom => new ClassroomGetDto
            {
                Id = classroom.Id,
                Name = classroom.Name,
                Description = classroom.Description,
                Channels = classroom.Channels.Select(x => new ClassroomChannelsGetDto
                {
                    Id = x.Channel.Id,
                    Name = x.Channel.Name,
                    Description = x.Channel.Description
                }).ToList(),
                Students = classroom.Students.Select(x => new ClassroomStudentsGetDto
                {
                    Id = x.Student.Id,
                    Name = x.Student.Name,
                    StudentEmail = x.Student.StudentEmail
                }).ToList()
            })
            .FirstOrDefault(classroom => classroom.Id == id);
        
        response.Data = data;
        return Ok(response);
    }
    
    [HttpPost]
    public IActionResult Create([FromBody] ClassroomCreateDto createDto)
    {
        var response = new Response();
        
        if(string.IsNullOrEmpty(createDto.Name))
        {
             response.AddError(nameof(createDto.Name), "Class Name must not be empty.");   
        }
        
        if(response.HasErrors)
        {
            return BadRequest(response);
        }
        var classroomToCreate = new Classroom
        {
            Name = createDto.Name,
            Description = createDto.Description,
        };
        
        _dataContext.Set<Classroom>().Add(classroomToCreate);
        _dataContext.SaveChanges();
        
        var classroomToReturn  = new ClassroomGetDto
        {
            Id = classroomToCreate.Id,
            Name = classroomToCreate.Name,
            Description = classroomToCreate.Description,
        };
        
        response.Data = classroomToReturn;
        return Created("", response);
    }
    
    [HttpPost("{classroomId}/student/{studentId}")]
    public IActionResult AddStudentToClassroom(int classroomId, int studentId)
    {
        var response = new Response();
        
        var classroom = _dataContext.Set<Classroom>()
            .FirstOrDefault(x => x.Id == classroomId);
        
        var student = _dataContext.Set<Student>()
            .FirstOrDefault(x => x.Id == studentId);
        
        if(classroom == null || student == null)
        {
            response.AddError("id", "Classroom or Student not found.");
        }
        
        var classroomStudents = new ClassroomStudents
        {
            Classroom = classroom,
            Student = student
        };
        
        _dataContext.Set<ClassroomStudents>().Add(classroomStudents);
        _dataContext.SaveChanges();
        
        response.Data = new ClassroomGetDto
        {
            Id = classroom.Id,
            Name = classroom.Name,
            Description = classroom.Description,
            Students = classroom.Students.Select(x => new ClassroomStudentsGetDto
            {
                Id = x.Student.Id,
                Name = x.Student.Name,
                StudentEmail = x.Student.StudentEmail
            }).ToList()
        };
        
        student.Classrooms.Add(classroom.Name);
        
        return Ok(response);
    }
    
    [HttpPost("{classroomId}/channel/{channelId}")]
    public IActionResult AddChannelToClassroom(int classroomId, int channelId)
    {
        var response = new Response();
        
        var classroom = _dataContext.Set<Classroom>()
            .FirstOrDefault(x => x.Id == classroomId);
        
        var channel = _dataContext.Set<Channel>()
            .FirstOrDefault(x => x.Id == channelId);
        
        if(classroom == null || channel == null)
        {
            response.AddError("id", "Classroom or Channel not found.");
        }
        
        var classroomChannels = new ClassroomChannels
        {
            Classroom = classroom,
            Channel = channel
        };
        
        _dataContext.Set<ClassroomChannels>().Add(classroomChannels);
        _dataContext.SaveChanges();
        
        response.Data = new ClassroomGetDto
        {
            Id = classroom.Id,
            Name = classroom.Name,
            Description = classroom.Description,
            Channels = classroom.Channels.Select(x => new ClassroomChannelsGetDto
            {
                Id = x.Channel.Id,
                Name = x.Channel.Name,
                Description = x.Channel.Description
            }).ToList()
        };
        
        return Ok(response);
    }
    
    [HttpPut("{id}")]
    public IActionResult Update([FromBody] ClassroomUpdateDto updateDto, int id)
    {
        var response = new Response();
        
        if(string.IsNullOrEmpty(updateDto.Name))
        {
             response.AddError(nameof(updateDto.Name), "Class Name must not be empty.");   
        }
        
        var classroomToUpdate = _dataContext.Set<Classroom>()
            .FirstOrDefault(classroom => classroom.Id == id);
        
        if(classroomToUpdate == null)
        {
            response.AddError("id", "Classroom not found.");
        }
        
        if(response.HasErrors)
        {
            return BadRequest(response);
        }
        
        classroomToUpdate.Name = updateDto.Name;
        classroomToUpdate.Description = updateDto.Description;
        
        _dataContext.SaveChanges();
        
        var classroomToReturn = new ClassroomGetDto
        {
            Id = classroomToUpdate.Id,
            Name = classroomToUpdate.Name,
            Description = classroomToUpdate.Description
        };
        
        response.Data = classroomToReturn;
        
        return Ok(response);
    }
    
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var response = new Response();
        
        var classroomToDelete = _dataContext.Set<Classroom>()
            .FirstOrDefault(classroom => classroom.Id == id);
        
        if(classroomToDelete == null)
        {
            response.AddError("id", "Classroom not found.");
        }
        
        if(response.HasErrors)
        {
            return BadRequest(response);
        }
            
        _dataContext.Set<Classroom>().Remove(classroomToDelete);
        _dataContext.SaveChanges();
        
        response.Data = true;
        return Ok(response);
    }
}