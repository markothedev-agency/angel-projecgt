import { ActionIcon, Text } from "@mantine/core";
import { useState, useEffect } from "react";
import {
  Container,
  Title,
  TextInput,
  Grid,
  Card,
  Tooltip,
  Button,
  Flex,
  Avatar,
  Loader
} from "@mantine/core";
import { FaHome, FaPlusCircle, FaSearch } from "react-icons/fa";
import { useNavigate } from "react-router-dom";
import api from "../../config/axios";
import { getInitials, useAuth } from "../../authentication/use-auth";
import { ServerDetail } from "./Server-detail";
import { FaTrash, FaEdit } from "react-icons/fa";
import { notifications, showNotification } from "@mantine/notifications";

export interface ServersGetDto {
  id: number;
  name: string;
  description: string;
  initials: string; // Add initials for the server icon
}

export const ServersListing = () => {
  // const [servers, setServers] = useState<ServersGetDto[]>([
  //   {
  //     id: 1,
  //     name: "Econs2020 Study Group",
  //     description: "A place to study together",
  //     initials: "ES",
  //   },
  //   { id: 2, name: "Chess Gambit Club", description: "For lovers of chess", initials: "CC" },
  //   { id: 3, name: "Coding Homework", description: "Share coding tips and tricks", initials: "CH" },
  //   { id: 4, name: "Calculus Prodigy", description: "Math Homework help", initials: "CP" },
  // ]);
  const {servers,setServers,user} = useAuth()
  const [filteredServers, setFilteredServers] = useState<ServersGetDto[]>(servers);
  const [searchQuery, setSearchQuery] = useState("");
  const [isloading,setIsloading] = useState(false);
  const [serverDetails,setServerDetails] = useState<null|number>(null)
  const navigate = useNavigate();

  useEffect(() => {
    setFilteredServers(servers);
  }, [servers]);

  const handleSearch = (query: string) => {
    setSearchQuery(query);
    const filtered = servers.filter(
      (server:any) =>
        server.name.toLowerCase().includes(query.toLowerCase()) ||
        server.description.toLowerCase().includes(query.toLowerCase())
    );
    setFilteredServers(filtered);
  };

  const handleServerClick = (serverId: number) => {
    // Navigate to the server's page
    navigate(`/Server-detail/${serverId}`);
  };
  const handleDelete = async(serverID:number)=>{
    //
    try{
      setIsloading(true)
      const resp = await api.delete(`/api/server/${serverID}`);
      const data = resp.data
      setServers([...servers.filter((d:any)=>d.id!==serverID)])
      showNotification({ message: "Server Deleted!", color: "red" });
      setIsloading(false)
    }catch{
      showNotification({ message: "Something went wrong!", color: "red" });

    }
  }
  const getAllServers = async ()=>{
    const response = await api.get('/api/server');
    setIsloading(true)
    // console.log({})
    // @ts-ignore
    setServers(response.data.data.map(item=>({
      ...item,initials:getInitials(item.name),
    })))
    setIsloading(false)
  }
  useEffect(()=>{
    getAllServers()
  },[])

  if(isloading){
    return <Loader />
  }
  return (
    <div>
          <Flex style={{ height: "100vh" }}>
      {/* Sidebar */}
      <Container
        style={{
          backgroundColor: "#2f3136",
          width: "80px",
          height: "100vh",
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
          padding: "10px",
        }}
      >
        {/* Home Button */}
        <Tooltip label="Home" position="right" withArrow>
          <Button
            style={{
              backgroundColor: "#5865f2",
              borderRadius: "50%",
              height: "60px",
              width: "60px",
              marginBottom: "20px",
            }}
            onClick={() => navigate("/")}
          >
            <FaHome size={30} color="white" />
          </Button>
        </Tooltip>

        {/* Server Icons */}
        {servers.map((server:any) => (
          <Tooltip key={server.id} label={server.name} position="right" withArrow>
            <Avatar
              radius="xl"
              size={50}
              style={{
                backgroundColor: "#7289da",
                color: "white",
                marginBottom: "15px",
                cursor: "pointer",
              }}
              onClick={() => handleServerClick(server.id)}
            >
              {server.initials}
            </Avatar>
          </Tooltip>
        ))}

        {/* Add Server Button */}
        <Tooltip label="Add Server" position="right" withArrow>
          <Button
            style={{
              backgroundColor: "#3ba55c",
              borderRadius: "50%",
              height: "60px",
              width: "60px",
              marginTop: "auto",
            }}
            onClick={() => navigate("/Add-server")}
          >
            <FaPlusCircle size={30} color="white" />
          </Button>
        </Tooltip>
      </Container>

      {/* Main Content */}
      <Container style={{ flex: 1, padding: "20px" }}>
                    <TextInput
                      leftSection={<FaSearch />}
                      placeholder="Search for a server"
                      value={searchQuery}
                      onChange={(e) => handleSearch(e.target.value)}
                      style={{ width: "100%", maxWidth: "400px", marginBottom: "20px" }}
                    />
                    <Title order={2} style={{ marginBottom: "20px", textAlign: "center" }}>
                      {filteredServers.length === 0 ? "No servers found" : "Servers"}
                    </Title>
                    <Grid>
                      {filteredServers.map((server) => (
                        <Grid.Col span={4} key={server.id}
                        style={{position:'relative',}}
                        
                        >
                          <Card shadow="sm" padding="lg"
                          onClick={e=>{
                            handleServerClick(server.id)
                          }}
                          style={{
                            cursor:'pointer',
                          }}
                          >
                            <Text style={{ fontWeight: 500 }}>{server.name}</Text>
                            <Text style={{ fontSize: "small", color: "dimmed" }}>{server.description}</Text>
                          </Card>

{
  user?.userName.toLowerCase() ==='admin'?
  <ActionIcon
  id={`delete${server.id}`}
  style={{position:'absolute','bottom':'15px','right':'10px'}}
  color="red" onClick={e => {
    handleDelete(server.id)
  }}>
<FaTrash />
</ActionIcon>:''
}

                        </Grid.Col>
                      ))}
                    </Grid>
                  </Container>

    </Flex>
    </div>
  );
};
