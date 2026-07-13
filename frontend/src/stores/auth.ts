import { defineStore } from "pinia";
import api from '@/api/axios'

interface User {
    id: number
    email: string
    role: 'Director' | 'ProjectManager' | 'Employee'
    isTemporaryPassword: boolean
}

export const useAuthStore = defineStore('auth', {

    state: () => ({
        user: null as User | null,
        isAuthenticated: false,
        isLoading: true
    }),

    getters: {
        isDirector: (state) => state.user?.role === 'Director',
        isProjectManager: (state) => state.user?.role === 'ProjectManager',
        isEmployee: (state) => state.user?.role === 'Employee',
        mustChangePassword: (state) => state.user?.isTemporaryPassword === true
    },
    
    actions: {

        async login(email: string, regPassword: string){
            const response = await api.post('/api/Account/login', { email, password: regPassword })
            this.user = response.data
            this.isAuthenticated = true
        },

        async logout() {
            await api.post('/api/Account/logout')
            this.user = null
            this.isAuthenticated = false
        },

        async checkAuth(){
            try{
                this.isLoading = true
                const response = await api.get('/api/Account/me')
                this.user = response.data
                this.isAuthenticated = true
            }
            catch(error){
                this.user = null
                this.isAuthenticated = false            
            }
            finally{
                this.isLoading = false
            }
        }
    }

})