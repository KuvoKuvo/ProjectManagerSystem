<script setup lang="ts">

import { ref, reactive, onMounted, computed } from 'vue'
import api from '@/api/axios'

const isLoading = ref(false)
const listLoading = ref(false)
const globalError = ref('')
const successMessage = ref('')

const employees = ref<any[]>([])
const searchQuery = ref('')

const generatedPassword = ref('')
const isCopied = ref(false)

const isEditMode = ref(false)
const currEditingId = ref<number | null>(null)

const form = reactive({
  firstName: '',
  lastName: '',
  middleName: '',
  email: '',
  role: 'Employee'
})

const fetchEmployees = async () => {
    try{
        listLoading.value = true
        const response = await api.get('/api/Employees')
        employees.value = response.data
    }
    catch (err: any){
        console.error('Failed to load employees list:', err)
        globalError.value = 'Could not load employees. Please refresh.'
    }
    finally {
        listLoading.value = false
    }
}

const filteredEmployees = computed(() => {
    const query = searchQuery.value.toLowerCase().trim()
    if (!query) return employees.value

    return employees.value.filter(emp => {
        return (
            emp.firstName.toLowerCase().includes(query) ||
            emp.lastName.toLowerCase().includes(query) ||
            (emp.middleName && emp.middleName.toLowerCase().includes(query)) ||
            emp.email.toLowerCase().includes(query)
        )
    })
})

const startEdit = (emp: any) => {
    isEditMode.value = true
    currEditingId.value = emp.id
    generatedPassword.value = ''
    globalError.value = ''
    successMessage.value = ''

    form.firstName = emp.firstName
    form.lastName = emp.lastName
    form.middleName = emp.middleName || ''
    form.email = emp.email
    form.role = 'Employee'
}

const cancelEdit = () => {
  isEditMode.value = false
  currEditingId.value = null
  resetForm()
}

const resetForm = () => {
  form.firstName = ''
  form.lastName = ''
  form.middleName = ''
  form.email = ''
  form.role = 'Employee'
}

const handleSubmit = async () => {
    try{
        isLoading.value = true
        globalError.value = ''
        successMessage.value = ''

        if(isEditMode.value && currEditingId.value !== null){

            // UPDATE process
            await api.put(`/api/Employees/${currEditingId.value}`, {
                id: currEditingId.value,
                firstName: form.firstName,
                lastName: form.lastName,
                middleName: form.middleName,
                email: form.email
            })

            successMessage.value = `Employee "${form.lastName} ${form.firstName}" was successfully updated!`
            isEditMode.value = false
            currEditingId.value = null
            resetForm()

        }
        else{

            // CREATE process
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
                successMessage.value = 'Employee registered successfully!'
                resetForm()
            }
        }

        await fetchEmployees()
    }
    catch (err: any){
        console.error('Request processing error:', err)
        if (err.response && err.response.data) {
            globalError.value = err.response.data.message || err.response.data || 'Validation failed.'
        } 
        else {
            globalError.value = 'Network error. Please check server status.'
        }
    }
    finally{
        isLoading.value = false
    }
}

const handleDelete = async (id: number, fullName: string) => {

    if (!confirm(`Are you sure you want to completely remove "${fullName}"? This action cannot be undone.`)) {
        return
    }

    try{
        listLoading.value = true
        globalError.value = ''
        successMessage.value = ''

        await api.delete(`/api/Employees/${id}`)
        successMessage.value = `Employee "${fullName}" has been successfully deleted.`

        if (isEditMode.value && currEditingId.value === id) {
            cancelEdit()
        }

        await fetchEmployees()
    }

    catch (err: any){
        console.error('Delete request failed:', err)
        if (err.response && err.response.data) {
            globalError.value = err.response.data.message || 'Could not delete employee.'
        } 
        else {
            globalError.value = 'Failed to delete employee. Check connection settings.'
        }
    }

    finally{
        listLoading.value = false
    }

}

const copyToClipboard = async () => {
    try {
        await navigator.clipboard.writeText(generatedPassword.value)
        isCopied.value = true
        setTimeout(() => { isCopied.value = false }, 2000)
    } 
    catch (err) {
        console.error('Clipboard action failed:', err)
    }
}

onMounted(() => {
  fetchEmployees()
})

</script>

<template>
  <div class="min-h-screen bg-slate-50 py-8 px-4 sm:px-6 lg:px-8">
    <div class="max-w-7xl mx-auto space-y-6">
      
      <div v-if="globalError" class="p-4 bg-rose-50 border border-rose-200 text-rose-700 rounded-2xl text-sm shadow-sm">
        ⚠️ {{ globalError }}
      </div>
      <div v-if="successMessage" class="p-4 bg-emerald-50 border border-emerald-200 text-emerald-700 rounded-2xl text-sm shadow-sm">
        ✅ {{ successMessage }}
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        
        <div class="lg:col-span-2 bg-white rounded-2xl shadow-sm border border-slate-100 overflow-hidden flex flex-col">
          <div class="p-6 border-b border-slate-100 bg-slate-900 text-white flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
            <div>
              <h1 class="text-xl font-bold tracking-tight">System Employees</h1>
              <p class="text-slate-400 text-xs mt-0.5">Manage users, adjust records, and view system actors</p>
            </div>
            <div class="w-full sm:w-64">
              <input 
                v-model="searchQuery" 
                type="text" 
                placeholder="Search by name or email..." 
                class="w-full rounded-xl border border-slate-700 bg-slate-800 text-white px-3 py-1.5 text-xs focus:border-emerald-500 focus:outline-none placeholder-slate-400"
              />
            </div>
          </div>

          <div class="flex-grow overflow-x-auto">
            <div v-if="listLoading && employees.length === 0" class="p-12 text-center text-slate-400">
              <p class="animate-pulse">Loading system participants list...</p>
            </div>
            
            <div v-else-if="filteredEmployees.length === 0" class="p-12 text-center text-slate-400">
              <p>No matching employees found.</p>
            </div>

            <table v-else class="w-full border-collapse text-left text-sm">
              <thead class="bg-slate-50 border-b border-slate-100 text-slate-500 uppercase tracking-wider font-bold text-xs">
                <tr>
                  <th class="px-6 py-4">Full Name</th>
                  <th class="px-6 py-4">Email Address</th>
                  <th class="px-6 py-4 text-right">Actions</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-slate-100 font-medium text-slate-700">
                <tr 
                  v-for="emp in filteredEmployees" 
                  :key="emp.id" 
                  class="hover:bg-slate-50/50 transition-colors"
                  :class="{'bg-emerald-50/20 border-l-4 border-l-emerald-500': currEditingId === emp.id}"
                >
                  <td class="px-6 py-4">
                    <div class="font-bold text-slate-900">{{ emp.lastName }} {{ emp.firstName }}</div>
                    <div class="text-xs text-slate-400 mt-0.5">Middle: {{ emp.middleName || '—' }} (ID: {{ emp.id }})</div>
                  </td>
                  <td class="px-6 py-4">
                    <span class="font-mono text-xs text-slate-600 bg-slate-100 px-2 py-1 rounded-md">{{ emp.email }}</span>
                  </td>
                  <td class="px-6 py-4 text-right">
                    <div class="flex items-center justify-end gap-2">
                      <button 
                        @click="startEdit(emp)"
                        class="px-3 py-1.5 bg-slate-100 hover:bg-slate-200 text-slate-700 rounded-lg text-xs font-bold transition-colors cursor-pointer"
                      >
                        Edit
                      </button>
                      <button 
                        @click="handleDelete(emp.id, `${emp.lastName} ${emp.firstName}`)"
                        class="px-3 py-1.5 bg-rose-50 hover:bg-rose-100 text-rose-600 rounded-lg text-xs font-bold transition-colors cursor-pointer"
                      >
                        Delete
                      </button>
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <div class="bg-white rounded-2xl shadow-sm border border-slate-100 p-6 flex flex-col justify-between">
          <div class="space-y-6">
            <div>
              <h2 class="text-lg font-bold text-slate-900">
                {{ isEditMode ? 'Edit Employee Data' : 'Register New Employee' }}
              </h2>
              <p class="text-xs text-slate-400 mt-0.5">
                {{ isEditMode ? 'Modify registration fields. Email update changes login ID.' : 'Creates a security profile and generates credentials.' }}
              </p>
            </div>

            <div v-if="generatedPassword" class="p-5 bg-amber-50 border-2 border-amber-300 rounded-xl space-y-3 shadow-sm">
              <div class="flex items-start gap-2.5">
                <span class="text-xl">⚠️</span>
                <div>
                  <h3 class="font-bold text-amber-950 text-xs">Security Credentials Issued</h3>
                  <p class="text-[11px] text-amber-800 mt-0.5">
                    Temporary Password: 
                    <span class="font-mono bg-white px-1.5 py-0.5 border border-amber-200 rounded font-bold text-slate-900 text-xs select-all block mt-1 text-center">
                      {{ generatedPassword }}
                    </span>
                  </p>
                </div>
              </div>
              <button 
                type="button" 
                @click="copyToClipboard" 
                class="w-full py-2 bg-amber-600 hover:bg-amber-700 text-white rounded-lg text-[11px] font-bold transition-colors flex justify-center items-center gap-1 cursor-pointer"
              >
                <span>{{ isCopied ? 'Copied!' : 'Copy to Clipboard' }}</span>
              </button>
            </div>

            <form @submit.prevent="handleSubmit" class="space-y-4">
              <div>
                <label class="block text-[11px] font-bold text-slate-500 uppercase tracking-wider mb-1">First Name *</label>
                <input v-model="form.firstName" type="text" required class="w-full rounded-xl border border-slate-200 px-3 py-2 text-sm focus:border-emerald-500 focus:outline-none bg-slate-50" />
              </div>

              <div>
                <label class="block text-[11px] font-bold text-slate-500 uppercase tracking-wider mb-1">Last Name *</label>
                <input v-model="form.lastName" type="text" required class="w-full rounded-xl border border-slate-200 px-3 py-2 text-sm focus:border-emerald-500 focus:outline-none bg-slate-50" />
              </div>

              <div>
                <label class="block text-[11px] font-bold text-slate-500 uppercase tracking-wider mb-1">Middle Name</label>
                <input v-model="form.middleName" type="text" class="w-full rounded-xl border border-slate-200 px-3 py-2 text-sm focus:border-emerald-500 focus:outline-none bg-slate-50" />
              </div>

              <div>
                <label class="block text-[11px] font-bold text-slate-500 uppercase tracking-wider mb-1">Email *</label>
                <input v-model="form.email" type="email" required class="w-full rounded-xl border border-slate-200 px-3 py-2 text-sm focus:border-emerald-500 focus:outline-none bg-slate-50" />
              </div>

              <div v-if="!isEditMode">
                <label class="block text-[11px] font-bold text-slate-500 uppercase tracking-wider mb-1">System Role *</label>
                <select v-model="form.role" required class="w-full rounded-xl border border-slate-200 px-3 py-2 text-sm focus:border-emerald-500 focus:outline-none bg-slate-50">
                  <option value="Employee">Employee</option>
                  <option value="ProjectManager">Project Manager</option>
                  <option value="Director">Director</option>
                </select>
              </div>

              <div class="space-y-2 pt-2">
                <button 
                  type="submit" 
                  :disabled="isLoading" 
                  class="w-full py-2.5 bg-emerald-600 hover:bg-emerald-500 text-white font-semibold text-sm rounded-xl transition-all shadow-md cursor-pointer disabled:opacity-50"
                >
                  <span v-if="isLoading">Processing API Request...</span>
                  <span v-else>{{ isEditMode ? 'Apply Updates' : 'Create Account' }}</span>
                </button>
                
                <button 
                  v-if="isEditMode"
                  type="button" 
                  @click="cancelEdit" 
                  class="w-full py-2.5 bg-slate-100 hover:bg-slate-200 text-slate-700 font-semibold text-xs rounded-xl transition-all cursor-pointer"
                >
                  Cancel Edit
                </button>
              </div>
            </form>
          </div>
        </div>

      </div>

    </div>
  </div>
</template>

