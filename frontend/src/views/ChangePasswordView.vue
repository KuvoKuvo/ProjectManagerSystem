<script setup lang="ts">

import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import api from '@/api/axios'

const router = useRouter()
const authStore = useAuthStore()

const currPassword = ref('')
const newPassword = ref('')
const confirmPassword = ref('')

const isLoading = ref(false)
const errorMessage = ref('')
const successMessage = ref('')

// Basic client-side validation
const isValid = computed(() => {
  return (
    currPassword.value.length > 0 &&
    newPassword.value.length >= 6 &&
    newPassword.value === confirmPassword.value
  )
})

const handleChangePassword = async () => {
    if (newPassword.value !== confirmPassword.value){
        errorMessage.value = 'New passwords do not match!'
        return
    }
    try{
        isLoading.value = true
        errorMessage.value = ''
        successMessage.value = ''

        await api.post('/api/Account/change-password', {
            currPassword: currPassword.value,
            newPassword: newPassword.value
        })

        successMessage.value = 'Password changed successfully! Redirecting...'

        await authStore.checkAuth()
        
        setTimeout(() => {
            router.push({name: 'dashboard'})
        }, 1500)
    }
    catch (error: any) {
        errorMessage.value = 
        error.response?.data?.message || 'Failed to change password. Ensure your current password is correct.'
    }
    finally {
        isLoading.value = false
    }
}

</script>

<template>
  <div class="flex min-h-screen items-center justify-center bg-slate-100 px-4 py-12 sm:px-6 lg:px-8">
    <div class="w-full max-w-md space-y-8 rounded-2xl bg-white p-8 shadow-xl">
      <div>
        <h2 class="mt-6 text-center text-2xl font-extrabold text-slate-900 tracking-tight">
          Password Change Required
        </h2>
        <p class="mt-2 text-center text-sm text-slate-600">
          You are logging in with a temporary password. Please set a secure password to continue.
        </p>
      </div>

      <form class="mt-8 space-y-4" @submit.prevent="handleChangePassword">
        <div>
          <label for="current-password" class="block text-sm font-medium text-slate-700 mb-1">
            Current Temporary Password
          </label>
          <input 
            id="current-password" 
            v-model="currPassword" 
            type="password" 
            required 
            class="block w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 placeholder-slate-400 focus:border-emerald-500 focus:outline-none focus:ring-emerald-500 sm:text-sm"
            placeholder="••••••••" 
          />
        </div>

        <div>
          <label for="new-password" class="block text-sm font-medium text-slate-700 mb-1">
            New Secure Password (min 6 characters)
          </label>
          <input 
            id="new-password" 
            v-model="newPassword" 
            type="password" 
            required 
            class="block w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 placeholder-slate-400 focus:border-emerald-500 focus:outline-none focus:ring-emerald-500 sm:text-sm"
            placeholder="••••••••" 
          />
        </div>

        <div>
          <label for="confirm-password" class="block text-sm font-medium text-slate-700 mb-1">
            Confirm New Password
          </label>
          <input 
            id="confirm-password" 
            v-model="confirmPassword" 
            type="password" 
            required 
            class="block w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 placeholder-slate-400 focus:border-emerald-500 focus:outline-none focus:ring-emerald-500 sm:text-sm"
            placeholder="••••••••" 
          />
          <p 
            v-if="newPassword && confirmPassword && newPassword !== confirmPassword" 
            class="text-xs text-red-600 mt-1"
          >
            ❌ Passwords do not match.
          </p>
        </div>

        <div v-if="errorMessage" class="text-sm font-medium text-red-600 bg-red-50 p-3 rounded-lg border border-red-200">
          ⚠️ {{ errorMessage }}
        </div>

        <div v-if="successMessage" class="text-sm font-medium text-emerald-600 bg-emerald-50 p-3 rounded-lg border border-emerald-200">
          ✅ {{ successMessage }}
        </div>

        <div>
          <button 
            type="submit" 
            :disabled="!isValid || isLoading"
            class="group relative flex w-full justify-center rounded-lg bg-emerald-600 px-4 py-2.5 text-sm font-semibold text-white shadow-md hover:bg-emerald-500 focus-visible:outline focus-visible:outline-emerald-600 transition-colors disabled:opacity-50 disabled:cursor-not-allowed cursor-pointer"
          >
            <span>{{ isLoading ? 'Updating password...' : 'Save Password & Continue' }}</span>
          </button>
        </div>
      </form>
    </div>
  </div>
</template>