import {
  Container,
  Title,
  Text,
  Button,
  Grid,
  Card,
  Badge,
  Flex,
  Modal,
  TextInput,
  Textarea,
  ActionIcon,
  Table,
  Group,
  Loader,
} from "@mantine/core";
import { useNavigate } from "react-router-dom";
import { createStyles } from "@mantine/emotion";
import { useEffect, useState } from "react";
import { FaTrash, FaEdit } from "react-icons/fa";
import { showNotification } from "@mantine/notifications";
import api from "../../config/axios";

// Define a type for the events
interface Event {
  id: number;
  name: string;
  date: string;
  description: string;
}

// This is the basic landing page with added components
export const LandingPage = () => {
  const { classes } = useStyles();
  const navigate = useNavigate();
  const [isloading,setIsloading] = useState(false);

  const [classrooms,setClassrooms] = useState<{name:string,description:string,id:number}[]>([]);
  // State for events management
  const [events, setEvents] = useState<Event[]>([
    { id: 1, name: "Integration Bee", date: "2024-12-12", description: "Compete and showcase your math skills" },
    { id: 2, name: "ACM-W Meeting", date: "2024-12-14", description: "Meeting at The Hub" },
    { id: 3, name: "CST Career Fair", date:"2024-11-28", description: "c" }
  ]);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [currentEvent, setCurrentEvent] = useState<Event | null>(null);
  const [formValues, setFormValues] = useState({ id:0,name: "",description: "" });

  // Handle form input changes
  const handleInputChange = (field: keyof Event, value: string) => {
    setFormValues({ ...formValues, [field]: value });
  };

  // Add or edit an event
  const handleAddEvent = async () => {
    if (!formValues.name  || !formValues.description) {
      showNotification({ message: "Please fill all fields", color: "red" });
      return;
    }
    if(currentEvent){
      try{
        setIsloading(true)
        const resp = await api.put(`/api/classrooms/${formValues.id}`,{
          name:formValues.name,
          description:formValues.description
        })
        setIsloading(false)
        setClassrooms(classrooms.map(item=>{
          if(item.id==formValues.id){
            console.log({formValues})
            return formValues
          }
          return item
        }))
        showNotification({ message: "Classroom updated", color: "success" });
      }catch{
        showNotification({ message: "Something went wrong", color: "error" });
        
      }
    }else{
      try{
        setIsloading(true)
        const resp:any = await api.post(`/api/classrooms/`,{
          name:formValues.name,
          description:formValues.description
        })
        setIsloading(false)
        setClassrooms([...classrooms,{
          ...resp.data.data
        }])
        showNotification({ message: "Classroom Created", color: "success" });
      }catch{
        showNotification({ message: "Something went wrong", color: "error" });
        
      }
    }




    // Reset form
    setFormValues({ name: "", id:0, description: "" });
    setIsModalOpen(false);
    setCurrentEvent(null);
  };

  // Edit an event
  const handleEditEvent = (event: any) => {
    setCurrentEvent(event);
    setFormValues(event);
    setIsModalOpen(true);
  };

  // Delete an event
  const handleDeleteEvent = async(id: number) => {
    
    try{
      setIsloading(true)
      const resp  =await api.delete(`/api/classrooms/${id}`)
      setClassrooms((prev) => prev.filter((event) => event.id !== id));
      showNotification({ message: "Classroom deleted", color: "teal" });    
      setIsloading(false)
       
    }catch{
      showNotification({ message: "Something went wrong", color: "red" });      
    }
  };
  const getClassrooms = async ()=>{
    try{
      setIsloading(true)
      const resp:any = await api.get(`/api/classrooms`);
      setClassrooms(resp.data.data)
      setIsloading(false)
    }catch{
      setIsloading(false)

    }
  }

  useEffect(()=>{
    getClassrooms()
  },[])
  if(isloading){
    return <Loader />
  }
  return (
    <Container className={classes.homePageContainer}>
      {/* Welcome Section */}
      <div
        style={{
          background: "linear-gradient(135deg, #6a11cb 0%, #2575fc 100%)", // Gradient background
          padding: "50px",
          borderRadius: "8px",
          textAlign: "center",
          color: "white",
        }}
      >
        <Title order={1}>Welcome to Class Connect!</Title>
        <Text size="lg" style={{ margin: "20px 0" }}>
          Connect with your peers, join servers, and stay updated with school events.
        </Text>
        <Button
          variant="filled"
          color="blue"
          size="lg"
          onClick={() => navigate("/servers")}
        >
          Browse Servers
        </Button>
      </div>

      {/* Main Content */}
      <Flex direction="column" gap="lg" style={{ marginTop: "30px" }}>
        {/* Upcoming Events Section */}
        <Card shadow="sm" padding="lg">
          <Flex justify="space-between" align="center" mb="lg">
            <Text fw={500}>School Classrooms</Text>
            <Button onClick={() => setIsModalOpen(true)}>Add Classroom</Button>
          </Flex>
          <Table striped highlightOnHover>
            <thead>
              <tr>
                <th>Name</th>
                {/* <th>Date</th> */}
                <th>Description</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {classrooms?.map((event) => (
                <tr key={event.id}>
                  <td>{event.name}</td>
                  {/* <td>{event.date}</td> */}
                  <td>{event.description}</td>
                  <td>
                    <Flex gap={"sm"}>
                      <ActionIcon color="blue" onClick={() => handleEditEvent(event)}>
                        <FaEdit />
                      </ActionIcon>
                      <ActionIcon color="red" onClick={() => handleDeleteEvent(event.id)}>
                        <FaTrash />
                      </ActionIcon>
                    </Flex>
                  </td>
                </tr>
              ))}
            </tbody>
          </Table>
        </Card>

        {/* Active Servers Section */}
        {/* <Card shadow="sm" padding="lg">
          <Text fw={500}>Active Servers</Text>
          <Grid>
            <Grid.Col span={4}>
              <Card shadow="sm">
                <Text>Server 1: Econs2020 Study Group</Text>
                <Text size="sm">Focus on group study and discussions.</Text>
                <Badge color="blue">Active</Badge>
              </Card>
            </Grid.Col>
            <Grid.Col span={4}>
              <Card shadow="sm">
                <Text>Server 2: Chess Gambit Club</Text>
                <Text size="sm">Collaborate on school projects.</Text>
                <Badge color="green">New</Badge>
              </Card>
            </Grid.Col>
          </Grid>
        </Card> */}

        {/* Call to Action */}
        <Flex justify="space-between" align="center">
          <Button variant="filled" onClick={() => navigate("/servers")}>
            Join a Server
          </Button>
          <Button variant="outline" onClick={() => navigate("/Add-server")}>
            Create a Server
          </Button>
        </Flex>
      </Flex>

      {/* Modal for Adding/Editing Events */}
      <Modal
        opened={isModalOpen}
        onClose={() => {
          setIsModalOpen(false);
          setCurrentEvent(null);
        }}
        title={currentEvent ? "Edit Event" : "Add Classroom"}
      >
        <TextInput
          label="Event Name"
          placeholder="Enter event name"
          value={formValues.name}
          onChange={(e) => handleInputChange("name", e.target.value)}
          required
          mb="md"
        />

        <Textarea
          label="Description"
          placeholder="Enter event description"
          value={formValues.description}
          onChange={(e) => handleInputChange("description", e.target.value)}
          required
          mb="md"
        />
        <Button fullWidth onClick={handleAddEvent}>
          {currentEvent ? "Update ClassRoom" : "Add Event"}
        </Button>
      </Modal>
    </Container>
  );
};

// Custom styles using Mantine's createStyles hook
const useStyles = createStyles((theme) => ({
  homePageContainer: {
    textAlign: "center",
    padding: theme.spacing.md,
  },
  homePageText: {
    fontFamily: theme.fontFamilyMonospace,
    fontSize: `calc(${theme.fontSizes.xl} * 1.5)`,
    fontWeight: 700,
  },
}));