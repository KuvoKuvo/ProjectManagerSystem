<script setup lang="ts">

import { ref, reactive, computed, watch } from 'vue'
import { useRouter } from 'vue-router'
import api from '@/api/axios'

const router = useRouter()

const currStep = ref(1)
const isLoading = ref(false)
const globalError = ref('')

const wizardData = reactive({

    name: '',
    startDate: '',
    endDate: '',
    priority: 1,

    customerCompany: '',
    executorCompany: '',

    projectManagerId: null as number | null,
    projectManagerName: '',

    employees: [] as Array<{ id: number, fullName: string }>,

    files: [] as File[]
})

const searchQuery = ref('')
const searchResult = ref<Array<{
    id: number,
    fullName: string
}>>([])
const isSearching = ref(false)
let debounceTimeout: ReturnType<typeof setTimeout>

const fetchEmployees = async (term: string) => {
    if(!term.trim()){
        searchResult.value = []
        return
    }
    try{
        isSearching.value = true
        const response = await api.get(`/api/Employees/search?searchTrim=${encodeURIComponent(term)}`)
        searchResult.value = response.data
    }
    catch (err){
        console.error('Employee search error:', err)
    }
    finally{
        isSearching.value = false
    }
}

watch(searchQuery, (newQuery => {
    clearTimeout(debounceTimeout)
    debounceTimeout = setTimeout(() =>{
        fetchEmployees(newQuery)
    }, 300)
}))
// VALIDATION OF STEPS
// Checking dates
const isStep1Valid = computed(() => {
    if (!wizardData.name || !wizardData.startDate || !wizardData.endDate || wizardData.priority < 1) {
        return false
    }
    const start = new Date(wizardData.startDate)
    const end = new Date(wizardData.endDate)
    return end >= start 
})

// Counterparties are required
const isStep2Valid = computed(() => {
  return wizardData.customerCompany.trim().length > 0 && wizardData.executorCompany.trim().length > 0
})

// A supervisor must be selected
const isStep3Valid = computed(() => wizardData.projectManagerId !== null)

// There must be at least 1 performer
const isStep4Valid = computed(() => wizardData.employees.length > 0)

const isNextButtonActive = computed(() => {
  if (currStep.value === 1) return isStep1Valid.value
  if (currStep.value === 2) return isStep2Valid.value
  if (currStep.value === 3) return isStep3Valid.value
  if (currStep.value === 4) return isStep4Valid.value
  return true 
})

// STEP CONTROL LOGIC
const nextStep = () => {
    if(currStep.value < 5 && isNextButtonActive.value){
        currStep.value++
        searchQuery.value = ''
        searchResult.value = []
    }
}

const prevStep = () =>{
    if(currStep.value > 1){
        currStep.value--
        searchQuery.value = ''
        searchResult.value = []
    }
}

// CHOOSING A MANAGER
const selectProjectManager = (emp: {
    id: number,
    fullName: string
}) => {
    wizardData.projectManagerId = emp.id
    wizardData.projectManagerName = emp.fullName
    searchQuery.value = ''
    searchResult.value = []
}

// MULTI-CHOICE OF EMPLOYEE
const toggleEmployee = (emp: {
    id: number,
    fullName: string
}) => {
    const index = wizardData.employees.findIndex(e => e.id === emp.id)
    if(index === -1){
        wizardData.employees.push(emp)
    }
    else {
        wizardData.employees.splice(index, 1)
    }
}

const removeEmployee = (id: number) => {
  wizardData.employees = wizardData.employees.filter(e => e.id !== id)
}

// HTML5 DRAG & DROP
const isDragActive = ref(false)

const onDragOver = () => { isDragActive.value = true }
const onDragLeave = () => { isDragActive.value = false }

const onDrop = (event: DragEvent) => {
    isDragActive.value = false
    if (event.dataTransfer?.files){
        for (let i = 0; i < event.dataTransfer.files.length; i++){
            const file = event.dataTransfer.files[i]
            if (file) {
                wizardData.files.push(file)
            }
        }
    }
}

const onFileSelect = (event: Event) => {
  const target = event.target as HTMLInputElement
  if (target.files) {
    for (let i = 0; i < target.files.length; i++) {
      const file = target.files[i]
      if (file) {
        wizardData.files.push(file)
      }
    }
  }
}

const removeFile = (index: number) => {
  wizardData.files.splice(index, 1)
}

// SENDING TO THE BACKEND
const submitProject = async () => {
    try{

        isLoading.value = true
        globalError.value = ''

        const formData = new FormData()
        formData.append('Name', wizardData.name)
        formData.append('StartDate', wizardData.startDate)
        formData.append('EndDate', wizardData.endDate)
        formData.append('Priority', wizardData.priority.toString())
        formData.append('CustomerCompany', wizardData.customerCompany)
        formData.append('ExecutorCompany', wizardData.executorCompany)
        formData.append('ProjectManagerId', wizardData.projectManagerId?.toString() || '')

        wizardData.employees.forEach((emp) => {
            formData.append('EmployeeIds', emp.id.toString())
        })

        wizardData.files.forEach((file) => {
            formData.append('UploadedFiles', file)
        })

        await api.post('/api/Projects', formData, {
            headers: { 'Content-Type': 'multipart/form-data' }
        })

        router.push({ name: 'dashboard' })

    }
    catch (err: any){
        globalError.value = err.response?.data?.message || 'An error occurred when creating the project.'
    }
    finally{
        isLoading.value = false
    }
}
</script>

<template>
  <div class="min-h-screen bg-slate-50 py-12 px-4 sm:px-6 lg:px-8">
    <div class="max-w-3xl mx-auto bg-white rounded-2xl shadow-xl overflow-hidden">
      
      <div class="bg-slate-900 px-8 py-6 text-white">
        <div class="flex justify-between items-center">
          <div>
            <h1 class="text-2xl font-bold tracking-tight">Creating a new project</h1>
            <p class="text-slate-400 text-sm mt-1">Fill out the wizard's step-by-step form</p>
          </div>
          <span class="text-emerald-400 font-mono text-lg font-bold">Step {{ currStep }} of 5</span>
        </div>
        
        <div class="w-full bg-slate-700 h-2 rounded-full mt-6 overflow-hidden">
          <div class="bg-emerald-500 h-full transition-all duration-300" :style="{ width: `${currStep * 20}%` }"></div>
        </div>
      </div>

      <div class="p-8">
        <div v-if="globalError" class="mb-6 p-4 bg-red-50 border border-red-200 text-red-700 rounded-xl text-sm">
          ⚠️ {{ globalError }}
        </div>

        <div v-if="currStep === 1" class="space-y-6">
          <h2 class="text-xl font-bold text-slate-800 border-b border-slate-100 pb-2">Step 1: Basic Information</h2>
          <div>
            <label class="block text-sm font-semibold text-slate-700 mb-1">Project name *</label>
            <input v-model="wizardData.name" type="text" class="w-full rounded-xl border border-slate-300 px-4 py-2.5 focus:border-emerald-500 focus:outline-none focus:ring-1 focus:ring-emerald-500" placeholder="Внедрение ERP системы" />
          </div>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-semibold text-slate-700 mb-1">Start date *</label>
              <input v-model="wizardData.startDate" type="date" class="w-full rounded-xl border border-slate-300 px-4 py-2.5 focus:border-emerald-500 focus:outline-none" />
            </div>
            <div>
              <label class="block text-sm font-semibold text-slate-700 mb-1">End date *</label>
              <input v-model="wizardData.endDate" type="date" class="w-full rounded-xl border border-slate-300 px-4 py-2.5 focus:border-emerald-500 focus:outline-none" />
              <p v-if="wizardData.startDate && wizardData.endDate && !isStep1Valid" class="text-xs text-red-600 mt-1">
                ⚠️ The end date cannot be earlier than the start date!
              </p>
            </div>
          </div>
          <div>
            <label class="block text-sm font-semibold text-slate-700 mb-1">Priority (number) *</label>
            <input v-model.number="wizardData.priority" type="number" min="1" class="w-full rounded-xl border border-slate-300 px-4 py-2.5 focus:border-emerald-500 focus:outline-none" />
          </div>
        </div>

        <div v-if="currStep === 2" class="space-y-6">
          <h2 class="text-xl font-bold text-slate-800 border-b border-slate-100 pb-2">Step 2: Counterparties</h2>
          <div>
            <label class="block text-sm font-semibold text-slate-700 mb-1">The customer company *</label>
            <input v-model="wizardData.customerCompany" type="text" class="w-full rounded-xl border border-slate-300 px-4 py-2.5 focus:border-emerald-500 focus:outline-none" placeholder="ООО Газпром Сбыт" />
          </div>
          <div>
            <label class="block text-sm font-semibold text-slate-700 mb-1">The executing company *</label>
            <input v-model="wizardData.executorCompany" type="text" class="w-full rounded-xl border border-slate-300 px-4 py-2.5 focus:border-emerald-500 focus:outline-none" placeholder="ПАО Иннотех Софт" />
          </div>
        </div>

        <div v-if="currStep === 3" class="space-y-6">
          <h2 class="text-xl font-bold text-slate-800 border-b border-slate-100 pb-2">Step 3: Project Manager</h2>
          
          <div v-if="wizardData.projectManagerId" class="p-4 bg-emerald-50 border border-emerald-200 rounded-xl flex justify-between items-center">
            <div>
              <p class="text-xs text-emerald-700 font-medium">Selected Supervisor:</p>
              <p class="text-base font-bold text-slate-800">{{ wizardData.projectManagerName }}</p>
            </div>
            <button @click="wizardData.projectManagerId = null" class="text-xs text-red-600 hover:underline cursor-pointer">Reset</button>
          </div>

          <div v-else class="relative">
            <label class="block text-sm font-semibold text-slate-700 mb-1">Employee search by full name *</label>
            <input v-model="searchQuery" type="text" class="w-full rounded-xl border border-slate-300 px-4 py-2.5 focus:border-emerald-500 focus:outline-none" placeholder="Start typing, for example: Ivanov..." />
            
            <div v-if="isSearching" class="absolute z-10 w-full bg-white border border-slate-200 rounded-xl mt-1 p-4 shadow-lg text-sm text-slate-500">
              🔍 Looking for employees on the server...
            </div>
            
            <ul v-if="searchResult.length > 0 && !isSearching" class="absolute z-10 w-full bg-white border border-slate-200 rounded-xl mt-1 shadow-lg max-h-60 overflow-y-auto divide-y divide-slate-100">
              <li v-for="emp in searchResult" :key="emp.id" @click="selectProjectManager(emp)"
                class="px-4 py-3 hover:bg-slate-50 cursor-pointer text-sm font-medium text-slate-700 transition-colors">
                👤 {{ emp.fullName }}
              </li>
            </ul>

            <div v-if="searchQuery && searchResult.length === 0 && !isSearching" class="absolute z-10 w-full bg-white border border-slate-200 rounded-xl mt-1 p-4 shadow-lg text-sm text-amber-600">
              No one was found for the query "{{ searchQuery }}"
            </div>
          </div>
        </div>

        <div v-if="currStep === 4" class="space-y-6">
          <h2 class="text-xl font-bold text-slate-800 border-b border-slate-100 pb-2">Step 4: Project Executors</h2>
          
          <div>
            <label class="block text-sm font-semibold text-slate-700 mb-2">Selected team (minimum 1 person) *</label>
            <div class="flex flex-wrap gap-2 p-3 bg-slate-50 rounded-xl min-h-[50px] border border-dashed border-slate-200">
              <span v-for="emp in wizardData.employees" :key="emp.id" 
                class="inline-flex items-center gap-1.5 bg-slate-900 text-white text-xs font-semibold px-3 py-1.5 rounded-full shadow-sm">
                {{ emp.fullName }}
                <button type="button" @click="removeEmployee(emp.id)" class="text-slate-400 hover:text-red-400 font-bold ml-1 focus:outline-none cursor-pointer">×</button>
              </span>
              <p v-if="wizardData.employees.length === 0" class="text-xs text-slate-400 self-center">The project team is empty. Add people via the search form below.</p>
            </div>
          </div>

          <div class="relative">
            <label class="block text-sm font-semibold text-slate-700 mb-1">Search and add participants</label>
            <input v-model="searchQuery" type="text" class="w-full rounded-xl border border-slate-300 px-4 py-2.5 focus:border-emerald-500 focus:outline-none" placeholder="Enter the employee's name..." />
            
            <div v-if="isSearching" class="absolute z-10 w-full bg-white border border-slate-200 rounded-xl mt-1 p-4 shadow-lg text-sm text-slate-500">
              🔍 Backend database search...
            </div>
            
            <ul v-if="searchResult.length > 0 && !isSearching" class="absolute z-10 w-full bg-white border border-slate-200 rounded-xl mt-1 shadow-lg max-h-60 overflow-y-auto divide-y divide-slate-100">
              <li v-for="emp in searchResult" :key="emp.id" @click="toggleEmployee(emp)"
                class="px-4 py-3 hover:bg-slate-50 cursor-pointer text-sm font-medium text-slate-700 flex justify-between items-center transition-colors">
                <span>👤 {{ emp.fullName }}</span>
                <span class="text-xs font-bold" :class="wizardData.employees.some(e => e.id === emp.id) ? 'text-rose-600' : 'text-emerald-600'">
                  {{ wizardData.employees.some(e => e.id === emp.id) ? 'Delete' : 'Add +' }}
                </span>
              </li>
            </ul>
          </div>
        </div>

        <div v-if="currStep === 5" class="space-y-6">
          <h2 class="text-xl font-bold text-slate-800 border-b border-slate-100 pb-2">Step 5: Attach documents</h2>
          
          <div 
            @dragover.prevent="onDragOver" 
            @dragleave.prevent="onDragLeave" 
            @drop.prevent="onDrop"
            :class="isDragActive ? 'border-emerald-500 bg-emerald-50/50' : 'border-slate-300 bg-slate-50 hover:bg-slate-100/50'"
            class="border-2 border-dashed rounded-2xl p-8 text-center transition-all duration-200 cursor-pointer relative group">
            
            <input type="file" multiple @change="onFileSelect" class="absolute inset-0 w-full h-full opacity-0 cursor-pointer" />
            
            <div class="space-y-2 pointer-events-none">
              <p class="text-3xl">📁</p>
              <p class="text-sm font-semibold text-slate-800 group-hover:text-emerald-600 transition-colors">
                Drag and drop the files here or click to select
              </p>
              <p class="text-xs text-slate-400">Any technical documents are supported.</p>
            </div>
          </div>

          <div v-if="wizardData.files.length > 0">
            <label class="block text-sm font-semibold text-slate-700 mb-2">Prepared files ({{ wizardData.files.length }})</label>
            <ul class="border border-slate-200 rounded-xl divide-y divide-slate-100 overflow-hidden bg-white shadow-sm">
              <li v-for="(file, idx) in wizardData.files" :key="idx" class="px-4 py-3 flex justify-between items-center text-sm">
                <div class="flex items-center gap-2 text-slate-700 font-medium truncate">
                  <span>📄</span>
                  <span class="truncate max-w-xs md:max-w-md">{{ file.name }}</span>
                  <span class="text-xs text-slate-400 font-mono">({{ (file.size / 1024).toFixed(1) }} КБ)</span>
                </div>
                <button type="button" @click="removeFile(idx)" class="text-xs text-rose-600 font-semibold hover:underline cursor-pointer">
                  Удалить
                </button>
              </li>
            </ul>
          </div>
        </div>

        <div class="mt-8 pt-6 border-t border-slate-100 flex justify-between items-center">
          <button 
            type="button" 
            @click="prevStep" 
            v-if="currStep > 1"
            :disabled="isLoading"
            class="px-5 py-2.5 border border-slate-300 rounded-xl text-slate-700 text-sm font-semibold hover:bg-slate-50 transition-colors cursor-pointer disabled:opacity-50">
            Back
          </button>
          <div v-else></div> <button 
            type="button" 
            @click="nextStep" 
            v-if="currStep < 5"
            :disabled="!isNextButtonActive"
            class="px-5 py-2.5 bg-slate-900 text-white rounded-xl text-sm font-semibold hover:bg-slate-800 transition-colors cursor-pointer disabled:opacity-40 disabled:cursor-not-allowed">
            Next →
          </button>

          <button 
            type="button" 
            @click="submitProject" 
            v-if="currStep === 5"
            :disabled="isLoading"
            class="px-6 py-2.5 bg-emerald-600 text-white rounded-xl text-sm font-semibold hover:bg-emerald-500 shadow-md transition-colors cursor-pointer disabled:opacity-50">
            <span v-if="isLoading">Creating a project...</span>
            <span v-else>🚀 Create a project</span>
          </button>
        </div>

      </div>
    </div>
  </div>
</template>