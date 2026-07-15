<script setup lang="ts">
import { useChangePassword } from '@/composables/useChangePassword'

const {
  currPassword,
  newPassword,
  confirmPassword,
  isLoading,
  errorMessage,
  successMessage,
  isValid,
  isPasswordMismatch,
  submitPasswordChange
} = useChangePassword()

</script>

<template>
  <div class="flex min-h-screen items-center justify-center bg-slate-100 px-4 py-12 sm:px-6 lg:px-8">
    <div class="w-full max-w-md space-y-8 rounded-2xl bg-white p-8 shadow-xl">
      <div>
        <h2 class="mt-6 text-center text-3xl font-extrabold text-slate-900 tracking-tight">
          Change Password
        </h2>
        <p class="mt-2 text-center text-sm text-slate-600">
          Provide your current password and choose a new secure password
        </p>
      </div>

      <form class="mt-8 space-y-6" @submit.prevent="submitPasswordChange">
        <div class="space-y-4 rounded-md shadow-sm">
          <div>
            <label for="current-password" class="block text-sm font-medium text-slate-700 mb-1">
              Current Password
            </label>
            <input 
              id="current-password" 
              v-model="currPassword" 
              type="password" 
              required 
              class="block w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 placeholder-slate-400 focus:border-emerald-500 focus:outline-none focus:ring-emerald-500 sm:text-sm"
            />
          </div>

          <div>
            <label for="new-password" class="block text-sm font-medium text-slate-700 mb-1">
              New Password (min 6 chars)
            </label>
            <input 
              id="new-password" 
              v-model="newPassword" 
              type="password" 
              required 
              class="block w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 placeholder-slate-400 focus:border-emerald-500 focus:outline-none focus:ring-emerald-500 sm:text-sm"
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
            />
            <p 
              v-if="isPasswordMismatch" 
              class="text-xs text-red-600 mt-1"
            >
              ❌ Passwords do not match.
            </p>
          </div>
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
            class="group relative flex w-full justify-center rounded-lg bg-emerald-600 px-4 py-2.5 text-sm font-semibold text-white shadow-md hover:bg-emerald-500 focus-visible:outline focus-visible:outline-emerald-600 disabled:opacity-50 disabled:cursor-not-allowed transition-all cursor-pointer"
          >
            <span v-if="isLoading">Processing API Request...</span>
            <span v-else>Update Password</span>
          </button>
        </div>
      </form>
    </div>
  </div>
</template>