import { useState } from "react";
import { Container, TextInput, Button, Loader } from "@mantine/core";
import { notifications } from "@mantine/notifications";
import api from "../../config/axios";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../authentication/use-auth";

export const AddServer = () => {
  const [name, setName] = useState("");
  const route = useNavigate()
  const [description, setDescription] = useState("");
  const [loading,setLoading] = useState(false)
  const {servers,setServers} = useAuth()
  const handleAddServer =async  () => {
    if (!name || !description) {
      notifications.show({
        title: "Error",
        message: "Please fill in all fields",
        color: "red",
      });
      return;
    }
    try{
      setLoading(true)
      const response = await api.post('/api/server/',{
        name,description
      });
      setLoading(false)

      notifications.show({
        title: "Success",
        message: `Server "${name}" added successfully!`,
        color: "green",
      });
      setServers([...servers,{
        "id": servers.length+1,
        "name":name,
        "description":description,
        "classes": []
      }])
      route('/Servers')
      
      console.log({response})
    }
    catch{
      notifications.show({
        title: "Error",
        message: `Something went wrong`,
        color: "red",
      });
    }


    
  };


  if(loading){
    return<Loader />
  }
  return (
    <Container>
      <TextInput
        label="Server Name"
        value={name}
        onChange={(e) => setName(e.target.value)}
        placeholder="Enter server name"
        required
      />
      <TextInput
        label="Server Description"
        value={description}
        onChange={(e) => setDescription(e.target.value)}
        placeholder="Enter server description"
        required
        mt="md"
      />
      <Button onClick={handleAddServer} mt="md">
        Add Server
      </Button>
    </Container>
  );
};