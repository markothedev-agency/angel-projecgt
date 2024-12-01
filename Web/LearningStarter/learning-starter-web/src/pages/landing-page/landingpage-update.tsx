import { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom"; // To navigate and get URL params
import { Button, TextInput, Textarea, Modal } from "@mantine/core";
import { showNotification } from "@mantine/notifications";

export const LandingPageUpdate = () => {
  const [formValues, setFormValues] = useState({
    name: "",
    date: "",
    description: "",
  });

  const { eventId } = useParams(); // Get the eventId from URL
  const navigate = useNavigate(); // For programmatic navigation

  useEffect(() => {
    if (eventId) {
      // If eventId exists, fetch the event data to edit
      // Example: Replace this with actual fetch logic from your state or API
      const eventToEdit = {
        id: 1,
        name: "Career Fair",
        date: "2024-12-05",
        description: "Career Fair for fall semester.",
      };

      setFormValues(eventToEdit);
    }
  }, [eventId]);

  // Handle input changes
  const handleInputChange = (field: string, value: string) => {
    setFormValues({ ...formValues, [field]: value });
  };

  // Handle form submission (either update or add)
  const handleSubmit = () => {
    // Validate form data
    if (!formValues.name || !formValues.date || !formValues.description) {
      showNotification({ message: "Please fill all fields", color: "red" });
      return;
    }

    // Update or add the event logic
    if (eventId) {
      // Update existing event
      showNotification({ message: "Event updated successfully", color: "green" });
    } else {
      // Add new event logic
      showNotification({ message: "Event added successfully", color: "green" });
    }

    // After submission, navigate back to the landing page
    navigate("/");
  };

  return (
    <div>
      <h1>{eventId ? "Edit Event" : "Add Event"}</h1>

      <TextInput
        label="Event Name"
        value={formValues.name}
        onChange={(e) => handleInputChange("name", e.target.value)}
        required
      />
      <TextInput
        label="Date"
        value={formValues.date}
        onChange={(e) => handleInputChange("date", e.target.value)}
        required
      />
      <Textarea
        label="Description"
        value={formValues.description}
        onChange={(e) => handleInputChange("description", e.target.value)}
        required
      />
      <Button onClick={handleSubmit}>{eventId ? "Update Event" : "Add Event"}</Button>
    </div>
  );
};