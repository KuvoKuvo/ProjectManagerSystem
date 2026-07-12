<script setup lang="ts">
import { useAuthStore } from '@/stores/auth'
import { useRouter } from 'vue-router'

const authStore = useAuthStore()
const router = useRouter()

const handleLogout = async () => {
  await authStore.logout()
  router.push({ name: 'login' })
}
</script>

<template>
  <div class="p-8 bg-slate-50 min-h-screen">
    <div class="max-w-4xl mx-auto bg-white p-6 rounded-xl shadow-md flex justify-between items-center">
      <div>
        <h1 class="text-2xl font-bold text-slate-800">Main Control Panel</h1>
        <p class="text-slate-600 mt-1">You entered as: <strong class="text-emerald-600">{{ authStore.user?.email }}</strong></p>
        <p class="text-xs text-slate-400">Role in the system: {{ authStore.user?.role }}</p>
      </div>
      <button @click="handleLogout" class="px-4 py-2 bg-rose-600 text-white rounded-lg hover:bg-rose-500 transition-colors cursor-pointer text-sm font-medium">
        Exit
      </button>
    </div>
  </div>
</template>