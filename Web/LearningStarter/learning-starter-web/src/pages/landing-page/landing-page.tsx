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
} from "@mantine/core";
import { useNavigate } from "react-router-dom";
import { createStyles } from "@mantine/emotion";
import { useState } from "react";
import { FaTrash, FaEdit } from "react-icons/fa";
import { showNotification } from "@mantine/notifications";

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

  // State for events management
  const [events, setEvents] = useState<Event[]>([
    { id: 1, name: "Integration Bee", date: "2024-12-12", description: "Compete and showcase your math skills" },
    { id: 2, name: "ACM-W Meeting", date: "2024-12-14", description: "Meeting at The Hub" },
    { id: 3, name: "CST Career Fair", date:"2024-11-28", description: "c" }
  ]);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [currentEvent, setCurrentEvent] = useState<Event | null>(null);
  const [formValues, setFormValues] = useState({ name: "", date: "", description: "" });

  // Handle form input changes
  const handleInputChange = (field: keyof Event, value: string) => {
    setFormValues({ ...formValues, [field]: value });
  };

  // Add or edit an event
  const handleAddEvent = () => {
    if (!formValues.name || !formValues.date || !formValues.description) {
      showNotification({ message: "Please fill all fields", color: "red" });
      return;
    }

    if (currentEvent) {
      // Edit existing event
      setEvents((prev) =>
        prev.map((event) =>
          event.id === currentEvent.id ? { ...event, ...formValues } : event
        )
      );
    } else {
      // Add new event
      setEvents((prev) => [...prev, { ...formValues, id: Date.now() }]);
    }

    // Reset form
    setFormValues({ name: "", date: "", description: "" });
    setIsModalOpen(false);
    setCurrentEvent(null);
  };

  // Edit an event
  const handleEditEvent = (event: Event) => {
    setCurrentEvent(event);
    setFormValues(event);
    setIsModalOpen(true);
  };

  // Delete an event
  const handleDeleteEvent = (id: number) => {
    setEvents((prev) => prev.filter((event) => event.id !== id));
    showNotification({ message: "Event deleted", color: "teal" });
  };

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
            <Text fw={500}>Upcoming School Events</Text>
            <Button onClick={() => setIsModalOpen(true)}>Add Event</Button>
          </Flex>
          <Table striped highlightOnHover>
            <thead>
              <tr>
                <th>Event Name</th>
                <th>Date</th>
                <th>Description</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {events.map((event) => (
                <tr key={event.id}>
                  <td>{event.name}</td>
                  <td>{event.date}</td>
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
        <Card shadow="sm" padding="lg">
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
            {/* More server cards */}
          </Grid>
        </Card>

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
        title={currentEvent ? "Edit Event" : "Add Event"}
      >
        <TextInput
          label="Event Name"
          placeholder="Enter event name"
          value={formValues.name}
          onChange={(e) => handleInputChange("name", e.target.value)}
          required
          mb="md"
        />
        <TextInput
          label="Date"
          placeholder="YYYY-MM-DD"
          value={formValues.date}
          onChange={(e) => handleInputChange("date", e.target.value)}
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
          {currentEvent ? "Update Event" : "Add Event"}
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