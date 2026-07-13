<script setup lang="ts">
import { ref, reactive } from 'vue'
import api from '@/api/axios'

const isLoading = ref(false)
const globalError = ref('')

// the state of the input form
const form = reactive({
  firstName: '',
  lastName: '',
  middleName: '',
  email: '',
  role: 'Employee'
})

// A state for storing the generated one-time password
const generatedPassword = ref('')
const isCopied = ref(false)

const copyToClipboard = async () => {
  try {
    await navigator.clipboard.writeText(generatedPassword.value)
    isCopied.value = true
    setTimeout(() => { isCopied.value = false }, 2000)
  } catch (err) {
    console.error('Couldnt copy', err)
  }
}

const handleSubmit = async () => {
  try {
    isLoading.value = true
    globalError.value = ''
    generatedPassword.value = ''

    const response = await api.post('/api/Employees', {
      firstName: form.firstName,
      lastName: form.lastName,
      middleName: form.middleName,
      email: form.email,
      role: form.role
    })

    if (response.data && response.data.temporaryPassword) {
      generatedPassword.value = response.data.temporaryPassword
      
      form.firstName = ''
      form.lastName = ''
      form.middleName = ''
      form.email = ''
    }
  } 
  catch (err: any) {
    if (err.response && err.response.data){
      globalError.value = err.response.data.message || err.response.data || 'Data validation error.'
    }
    else{
      globalError.value = 'Couldnt contact the server. Check the network.'
    }
  } 
  finally {
    isLoading.value = false
  }
}
</script>

<template>
  <div class="min-h-screen bg-slate-50 py-12 px-4 sm:px-6 lg:px-8">
    <div class="max-w-md mx-auto bg-white rounded-2xl shadow-xl overflow-hidden">
      
      <div class="bg-slate-900 px-8 py-6 text-white">
        <h1 class="text-xl font-bold tracking-tight">Employee registration</h1>
        <p class="text-slate-400 text-xs mt-1">Adding a new participant to the project management system</p>
      </div>

      <div class="p-8 space-y-6">
        
        <div v-if="globalError" class="p-4 bg-red-50 border border-red-200 text-red-700 rounded-xl text-sm">
          ⚠️ {{ globalError }}
        </div>

        <div v-if="generatedPassword" class="p-5 bg-amber-50 border-2 border-amber-300 rounded-xl space-y-3 shadow-sm animate-fade-in">
          <div class="flex items-start gap-2.5">
            <span class="text-xl">⚠️</span>
            <div>
              <h3 class="font-bold text-amber-950 text-sm">Important Security Notice</h3>
              <p class="text-xs text-amber-800 mt-0.5">
                Employee created, password: <span class="font-mono bg-white px-1.5 py-0.5 border border-amber-200 rounded font-bold text-slate-900 text-sm select-all">{{ generatedPassword }}</span>. 
                Copy it, this message will no longer appear.!
              </p>
            </div>
          </div>
          
          <button 
            type="button" 
            @click="copyToClipboard" 
            class="w-full py-2 bg-amber-600 hover:bg-amber-700 text-white rounded-lg text-xs font-bold transition-colors flex justify-center items-center gap-1 cursor-pointer">
            <span>{{ isCopied ? 'Copied!' : 'Copy the password to the clipboard' }}</span>
          </button>
        </div>

        <form @submit.prevent="handleSubmit" class="space-y-4">
          <div>
            <label class="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1">FirstName *</label>
            <input v-model="form.firstName" type="text" required class="w-full rounded-xl border border-slate-300 px-4 py-2.5 focus:border-emerald-500 focus:outline-none text-sm" placeholder="Иван" />
          </div>

          <div>
            <label class="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1">LastName *</label>
            <input v-model="form.lastName" type="text" required class="w-full rounded-xl border border-slate-300 px-4 py-2.5 focus:border-emerald-500 focus:outline-none text-sm" placeholder="Иванов" />
          </div>

          <div>
            <label class="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1">MiddleName</label>
            <input v-model="form.middleName" type="text" class="w-full rounded-xl border border-slate-300 px-4 py-2.5 focus:border-emerald-500 focus:outline-none text-sm" placeholder="Иванович" />
          </div>

          <div>
            <label class="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1">Email *</label>
            <input v-model="form.email" type="email" required class="w-full rounded-xl border border-slate-300 px-4 py-2.5 focus:border-emerald-500 focus:outline-none text-sm" placeholder="ivanov@company.com" />
          </div>

          <div>
            <label class="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1">System Role *</label>
            <select v-model="form.role" required class="w-full rounded-xl border border-slate-300 px-4 py-2.5 focus:border-emerald-500 focus:outline-none text-sm bg-white">
              <option value="Employee">Employee</option>
              <option value="ProjectManager">Project Manager</option>
              <option value="Director">Director</option>
            </select>
          </div>

          <button 
            type="submit" 
            :disabled="isLoading" 
            class="w-full mt-6 py-3 bg-emerald-600 hover:bg-emerald-500 text-white font-semibold text-sm rounded-xl transition-all duration-150 shadow-md cursor-pointer disabled:opacity-50">
            <span v-if="isLoading">Account Generation...</span>
            <span v-else>Create an employee</span>
          </button>
        </form>

      </div>
    </div>
  </div>
</template>