import api from './axios';
import type { Employee, EmployeeCreatePayload } from './types';

export const EmployeesService = {

    // Get a list of all employees 
    async getAll(): Promise<Employee[]> {
        const response = await api.get<Employee[]>('/api/Employees');
        return response.data;
    },

    // Search for employees by text query
    async search(term: string): Promise<Employee[]> {
        if (!term.trim()) return [];
        const response = await api.get<Employee[]>(`/api/Employees/search`, {
        params: { term }
        });
        return response.data;
    },

    // Get a list of available project managers
    async getManagers(): Promise<Employee[]> {
        const response = await api.get<Employee[]>('/api/Employees/managers');
        return response.data;
    },

    // Create a new employee (returns an object with a temporary password)
    async create(payload: EmployeeCreatePayload): Promise<{ temporaryPassword?: string } & Employee> {
        const response = await api.post('/api/Employees', payload);
        return response.data;
    },

    // Update employee details
    async update(id: number, payload: EmployeeCreatePayload): Promise<void> {
        await api.put(`/api/Employees/${id}`, { id, ...payload });
    },

    // Remove employee
    async delete(id: number): Promise<void> {
        await api.delete(`/api/Employees/${id}`);
    }
};