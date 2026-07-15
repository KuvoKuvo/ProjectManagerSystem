<script setup lang="ts">
import { useLogin } from '@/composables/useLogin'

const {
  email,
  password,
  errorMessage,
  isLoading,
  handleLogin
} = useLogin()
</script>

<template>
  <div class="flex min-h-screen items-center justify-center bg-slate-100 px-4 py-12 sm:px-6 lg:px-8">
    <div class="w-full max-w-md space-y-8 rounded-2xl bg-white p-8 shadow-xl">
      <div>
        <h2 class="mt-6 text-center text-3xl font-extrabold text-slate-900 tracking-tight">
          Project Manager System
        </h2>
        <p class="mt-2 text-center text-sm text-slate-600">
          Log in to your account
        </p>
      </div>

      <form class="mt-8 space-y-6" @submit.prevent="handleLogin">
        <div class="space-y-4 rounded-md shadow-sm">
          <div>
            <label for="email-address" class="block text-sm font-medium text-slate-700 mb-1">
              Email Address
            </label>
            <input 
              id="email-address" 
              v-model="email" 
              type="email" 
              autocomplete="email" 
              required 
              class="relative block w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 placeholder-slate-400 focus:z-10 focus:border-emerald-500 focus:outline-none focus:ring-emerald-500 sm:text-sm" 
              placeholder="admin@projectmanager.com" 
            />
          </div>
          <div>
            <label for="password" class="block text-sm font-medium text-slate-700 mb-1">
              Password
            </label>
            <input 
              id="password" 
              v-model="password" 
              type="password" 
              autocomplete="current-password" 
              required 
              class="relative block w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 placeholder-slate-400 focus:z-10 focus:border-emerald-500 focus:outline-none focus:ring-emerald-500 sm:text-sm" 
              placeholder="••••••••" 
            />
          </div>
        </div>

        <div v-if="errorMessage" class="text-sm font-medium text-red-600 bg-red-50 p-3 rounded-lg border border-red-200">
          ⚠️ {{ errorMessage }}
        </div>

        <div>
          <button 
            type="submit" 
            :disabled="isLoading"
            class="group relative flex w-full justify-center rounded-lg bg-emerald-600 px-4 py-2.5 text-sm font-semibold text-white shadow-md hover:bg-emerald-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-emerald-600 disabled:opacity-50 transition-all cursor-pointer"
          >
            <span v-if="isLoading">Signing in...</span>
            <span v-else>Sign In</span>
          </button>
        </div>
      </form>
    </div>
  </div>
</template>