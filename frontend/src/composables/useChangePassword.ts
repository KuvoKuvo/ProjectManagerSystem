import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { AccountService } from '@/api/account.service'

export function useChangePassword() {
    const router = useRouter()
    const authStore = useAuthStore()

    const currPassword = ref('')
    const newPassword = ref('')
    const confirmPassword = ref('')

    const isLoading = ref(false)
    const errorMessage = ref('')
    const successMessage = ref('')

    const isValid = computed(() => 
        currPassword.value.length > 0 &&
        newPassword.value.length >= 6 &&
        newPassword.value === confirmPassword.value      
    )

    const isPasswordMismatch = computed(() => 
        newPassword.value.length > 0 &&
        confirmPassword.value.length > 0 &&
        newPassword.value !== confirmPassword.value
    )

    const submitPasswordChange = async () => {
        if(newPassword.value !== confirmPassword.value){
            errorMessage.value = 'New passwords do not match!'
            return
        }
        try{
            isLoading.value = true
            errorMessage.value = ''
            successMessage.value = ''
            
            await AccountService.changePassword({
                currPassword: currPassword.value,
                newPassword: newPassword.value
            })

            successMessage.value = 'Password changed successfully! Redirecting...'

            await authStore.checkAuth()

            setTimeout(() => {
                router.push({ name: 'dashboard' })
            }, 1500)

        }
        catch (error: any) {
            console.error('Password change error:', error)
            errorMessage.value = error.response?.data?.message || 'Failed to change password. Please try again.'
        }
        finally{
            isLoading.value = false
        }
    }
    return {
        currPassword,
        newPassword,
        confirmPassword,
        isLoading,
        errorMessage,
        successMessage,
        isValid,
        isPasswordMismatch,
        submitPasswordChange
    }
}