import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

export function useLogin() {
    const router = useRouter()
    const authStore = useAuthStore()

    const email = ref('')
    const password = ref('')
    const errorMessage = ref('')
    const isLoading = ref(false)

    const handleLogin = async () => {
        if (!email.value || !password.value){
            errorMessage.value = 'Fill in all the fields!'
            return
        }
        try{
            isLoading.value = true
            errorMessage.value = ''
            await authStore.login(email.value, password.value)
            router.push({ name: 'dashboard' })
        }
        catch (error: any){
            console.error('Login failed:', error)
            errorMessage.value = 
            error.response?.data?.message || 'Invalid username or password'
        }
        finally{
            isLoading.value = false
        }
    }
    return {
        email,
        password,
        errorMessage,
        isLoading,
        handleLogin
    }
}