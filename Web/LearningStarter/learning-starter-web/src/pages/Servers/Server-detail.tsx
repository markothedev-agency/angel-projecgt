import { useParams } from "react-router-dom";
import { Container, Flex, Text, TextInput, Button, ScrollArea, Box, Loader } from "@mantine/core";
import { useState, useEffect } from "react";
import api from "../../config/axios";
import { useAuth } from "../../authentication/use-auth";
import { notifications } from "@mantine/notifications";
import { EnvVars } from "../../config/env-vars";

interface Channel {
  id: number;
  name: string;
}

interface Message {
postID:number,
sentBy:string,
text:string,
time:string,
}

const channels: Channel[] = [
  { id: 1, name: "General" },
  { id: 2, name: "Homework Help" },
  { id: 3, name: "Projects" },
];

export const ServerDetail = () => {
  const { id } = useParams();
  const {user} = useAuth()
  const [messages, setMessages] = useState<Message[]>([]);
  const [currentMessage, setCurrentMessage] = useState("");
  const [selectedChannel, setSelectedChannel] = useState<Channel | null>(channels[0]);
  const [socket, setSocket] = useState<any>(null);
  const [loading,setLoading] = useState(false)

  const sendMessage = () => {

    if(!socket) return
    if (currentMessage.trim()) {
      socket.send(currentMessage.trim())
      // setMessages([
      //   ...messages,
      //   { id: Date.now(), sender: "You", content: currentMessage.trim() },
      // ]);
      setCurrentMessage("");
    }
  };

  const handleChannelClick = (channel: Channel) => {
    setSelectedChannel(channel);
  };
  const getServerMessages =async()=>{
    
    try{
      setLoading(true)
      const resp = await api.get(`/api/posts/get-server/${id}`)
      setLoading(false)
      // @ts-ignore
      if(resp?.data?.data?.posts?.length!==0){
      // @ts-ignore
        setMessages(resp.data.data.posts)
      }else{
        notifications.show({
          title: "No Message",
          message: "No Message Found!",
          color: "red",
        });
      }
      
    }catch{
      //
    }
  }

  useEffect(() => {
    getServerMessages()
  }, []);


  useEffect(()=>{


    const ws = new WebSocket(`${EnvVars.apiBaseUrl}/ws/${id}/${user?.userName}`); 
    // Event listeners
    ws.onopen = () => {
      console.log('Connected to WebSocket server');
      // ws.send(JSON.stringify({ type: 'hello', data: 'Hello Server!' })); 
      setSocket(ws)
    };

    ws.onmessage = (event) => {
      console.log({event})
      const wsMessage =JSON.parse(event.data)
      console.log('Received:',wsMessage, );
      setMessages((prevMessages) => [...prevMessages, {
        postID:wsMessage.postId,
        'sentBy':wsMessage.sentBy,
        text:wsMessage.message,
        time:''
      }]);
    };

    ws.onerror = (error) => {
      console.error('WebSocket Error:', error);
    };

    ws.onclose = () => {
      console.log('WebSocket connection closed');
    };

    // Cleanup on unmount
    return () => {
      ws.close();
    };
  },[])



  




  if(loading){
    return <Loader />
  }

  return (
    <Flex style={{ height: "100vh", backgroundColor: "#2f3136" }}>
      {/* Sidebar for channels */}
      <Box
        style={{
          backgroundColor: "#202225",
          width: "250px",
          padding: "10px",
          color: "#fff",
        }}
      >
        <Text fw={700} size="lg" mb="md">
          Channels
        </Text>
        {channels.map((channel) => (
          <Text
            key={channel.id}
            onClick={() => handleChannelClick(channel)}
            style={{
              padding: "10px",
              borderRadius: "5px",
              cursor: "pointer",
              backgroundColor: selectedChannel?.id === channel.id ? "#7289da" : "#2f3136",
              color: selectedChannel?.id === channel.id ? "#fff" : "#bbb",
              transition: "background-color 0.2s",
            }}
          >
            #{channel.name}
          </Text>
        ))}
      </Box>

      {/* Main Content Area */}
      <Container
        style={{
          flex: 1,
          padding: "20px",
          display: "flex",
          flexDirection: "column",
        }}
      >
        {/* Fancy Header */}
        <Flex justify="space-between" align="center" mb="lg" style={{ color: "#fff" }}>
          <Text fw={700} size="xl">
            Server: {id}
          </Text>
          <Button color="blue">Server Settings</Button>
        </Flex>

        {/* Chat Area */}
        <ScrollArea style={{ flex: 1, marginBottom: "10px", backgroundColor: "#36393f" }}>
          {messages.map((message) => (
            <Box
              key={message.postID}
              style={{
                padding: "10px",
                borderBottom: "1px solid #444",
                color: "#fff",
              }}
            >
              <Text fw={500}>{message.sentBy}</Text>
              <Text>{message.text}</Text>
            </Box>
          ))}
        </ScrollArea>

        {/* Message Input */}
        <Flex align="center" style={{ marginTop: "auto" }}>
          <TextInput
            style={{ flex: 1, marginRight: "10px" }}
            placeholder="Type your message here..."
            value={currentMessage}
            onChange={(e) => setCurrentMessage(e.target.value)}
          />
          <Button onClick={sendMessage} color="green">
            Send
          </Button>
        </Flex>
      </Container>
    </Flex>
  );
};