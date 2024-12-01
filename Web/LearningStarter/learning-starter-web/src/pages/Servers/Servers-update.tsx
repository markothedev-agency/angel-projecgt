import { Container, Title, TextInput } from "@mantine/core";
import { useForm } from "@mantine/form";
import { showNotification } from "@mantine/notifications"
import { useEffect, useState } from "react"
import { useNavigate, useParams } from "react-router-dom"
import api from "../../config/axios";
import { 
    ApiResponse, 
    ServersCreateUpdateDto,
    ServersGetDto, 
} from "../../constants/types"
import { routes } from "../../routes";

export const ServersUpdate = () => {
    const [Servers, setServers] = useState<ServersGetDto>();
    const navigate = useNavigate();
    const {id} = useParams();

    const mantineForm = useForm<ServersCreateUpdateDto>({
        initialValues:  Servers
    });

    useEffect(() => {
        fetchServers();

        async function fetchServers() {
            const response = await api.get<ApiResponse<ServersGetDto>>(
                `/api/Servers/${id}`
            );

            if(response.data.hasErrors) {
                showNotification ({message: "This Server cannot be found", color: "red"})
            }

            if (response.data.data) {
                setServers (response.data.data);
                mantineForm.setValues(response.data.data)
                mantineForm.resetDirty();
            }
        }
    }, [id]);

    const submitServers = async (values: ServersCreateUpdateDto) => {
        const response = await api.put<ApiResponse<ServersGetDto>>(
            `/api/Servers/${id}`,
             values
            );

        if (response.data.hasErrors) {
            showNotification ({ message: "There was an error loading this Server", color: "red"});
        }

        if (response.data.data) {
         navigate(routes.ServersListing)   ;
        }
    };

    return (
        <Container>
            {Servers && (
            <form onSubmit={mantineForm.onSubmit(submitServers)}>
                <TextInput
                 {...mantineForm.getInputProps("name")}
                 label="Name"
                 withAsterisk
                />
                <TextInput
                {...mantineForm.getInputProps("description")}
                label="Description"
                withAsterisk
                />            
            </form>
            )}
            <Title h={32}> Update Page {id} </Title>
        </Container>
   );
};
