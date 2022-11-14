import axios, { AxiosResponse } from "axios";
import IField from "../interfaces/IField";
import IListItem from "../interfaces/IListItem";
import ITemplate from "../interfaces/ITemplate";

export const getUploadToken = async (): Promise<string> => {
    const resp: AxiosResponse = await axios.get("/api/uploadtoken");
    return resp.data;
};

export const getLists = async (): Promise<IListItem[]> => {
    const resp: AxiosResponse = await axios.get("/api/lists");
    return resp.data;
};

export const getTemplates = async (): Promise<ITemplate[]> => {
    const resp: AxiosResponse = await axios.get("/api/templates");
    return resp.data;
};

export const createTemplate = async (data: ITemplate): Promise<ITemplate> => {
    const resp: AxiosResponse = await axios.post("/api/templates", data);
    return resp.data;
};

export const deleteTemplate = async (id: string): Promise<void> => {
    return await axios.delete(`/api/templates/${id}`);
};

export const generateDocument = async (fields: IField[], blobName: string): Promise<Blob> => {
    const resp: AxiosResponse = await axios.post("/api/generate", { fields, blobName }, { responseType: "blob" });
    return resp.data;
};
