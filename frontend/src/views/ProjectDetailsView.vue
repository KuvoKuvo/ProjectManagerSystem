<script setup lang="ts">

import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import api from '@/api/axios'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const projectId = Number(route.params.id)

const isLoading = ref(true)
const isUploading = ref(false)
const errorMessage = ref('')
const project = ref<any>(null)

const fileInput = ref<HTMLInputElement | null>(null)

const fetchProjectDetails = async () => {
    try{
        isLoading.value = true
        errorMessage.value = ''
        const response = await api.get(`/api/Projects/${projectId}`)
        project.value = response.data
    }
    catch(err: any){
        console.error('Failed to load project details:', err)
        errorMessage.value = err.response?.data?.message || 'Failed to load project details.'
    }
    finally{
        isLoading.value = false
    }
}

const downloadDocument = async (docId: number, fileName: string) => {
    try{
        const response = await api.get(`/api/projects/${projectId}/documents/${docId}`, {
            responseType: 'blob'
        })

        const url = window.URL.createObjectURL(new Blob([response.data]))
        const link = document.createElement('a')
        link.href = url
        link.setAttribute('download', fileName)
        document.body.appendChild(link)
        link.click()

        link.parentNode?.removeChild(link)
        window.URL.revokeObjectURL(url)
    }
    catch(err){
        console.error('Failed to download document:', err)
        alert('Failed to download document.')
    }
}

const handleFileUpload = async (event: Event) => {
    const target = event.target as HTMLInputElement
    if (!target.files || target.files.length === 0) return

    const file = target.files[0]
    if (!file) return

    try{
        isUploading.value = true
        const formData = new FormData()
        formData.append('file', file)

        await api.post(`/api/projects/${projectId}/documents`, formData, {
            headers: { 'Content-Type': 'multipart/form-data' }
        })

        target.value = ''
        await fetchProjectDetails()

    }
    catch(err: any){
        console.error('Failed to upload document:', err)
        alert(err.response?.data?.message || 'Failed to upload document.')
    }
    finally{
        isUploading.value = false
    }
}

const projectEmployeesList = computed(() => {
  return project.value?.assignedEmployees || []
})

const projectDocumentsList = computed(() => {
  return project.value?.documents || []
})

onMounted(() => {
  fetchProjectDetails()
})

</script>

<template>
  <div class="min-h-screen bg-slate-50 py-8 px-4 sm:px-6 lg:px-8">
    <div class="max-w-7xl mx-auto space-y-6">
      
      <div class="flex justify-between items-center">
        <button 
          @click="router.push({ name: 'dashboard' })" 
          class="inline-flex items-center gap-2 text-sm font-semibold text-slate-600 hover:text-slate-900 transition-colors cursor-pointer"
        >
          ← Back to Dashboard
        </button>
      </div>

      <div v-if="isLoading" class="bg-white rounded-2xl p-12 text-center border border-slate-100 shadow-sm">
        <p class="text-slate-500 animate-pulse text-lg">Loading project details...</p>
      </div>

      <div v-else-if="errorMessage" class="bg-red-50 border border-red-200 text-red-700 rounded-2xl p-8 text-center shadow-sm">
        <p class="text-lg font-semibold">⚠️ Error Loading Project</p>
        <p class="text-sm mt-1">{{ errorMessage }}</p>
      </div>

      <div v-else class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        
        <div class="lg:col-span-2 space-y-6">
          
          <div class="bg-white rounded-2xl p-6 md:p-8 shadow-sm border border-slate-100 space-y-6">
            <div class="flex flex-col md:flex-row md:items-center justify-between gap-4 border-b border-slate-100 pb-5">
              <div>
                <span 
                  class="inline-flex items-center px-2.5 py-1 rounded-full text-xs font-bold mb-2"
                  :class="{
                    'bg-slate-100 text-slate-800': project.priority === 1,
                    'bg-blue-50 text-blue-700': project.priority === 2,
                    'bg-amber-50 text-amber-700': project.priority === 3,
                    'bg-orange-50 text-orange-700': project.priority === 4,
                    'bg-red-50 text-red-700': project.priority >= 5,
                  }"
                >
                  Priority Level {{ project.priority }}
                </span>
                <h1 class="text-3xl font-extrabold text-slate-900 tracking-tight">{{ project.name }}</h1>
              </div>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div class="space-y-4">
                <h3 class="text-sm font-bold text-slate-400 uppercase tracking-wider">Counterparties</h3>
                <div class="bg-slate-50 rounded-xl p-4 space-y-3">
                  <div>
                    <p class="text-xs text-slate-500">Customer Company</p>
                    <p class="font-bold text-slate-800">{{ project.customerCompany }}</p>
                  </div>
                  <div class="border-t border-slate-200/60 pt-2">
                    <p class="text-xs text-slate-500">Executing Company</p>
                    <p class="font-bold text-slate-800">{{ project.executorCompany }}</p>
                  </div>
                </div>
              </div>

              <div class="space-y-4">
                <h3 class="text-sm font-bold text-slate-400 uppercase tracking-wider">Project Timeline</h3>
                <div class="bg-slate-50 rounded-xl p-4 space-y-3">
                  <div>
                    <p class="text-xs text-slate-500">Start Date</p>
                    <p class="font-bold text-slate-800">{{ new Date(project.startDate).toLocaleDateString() }}</p>
                  </div>
                  <div class="border-t border-slate-200/60 pt-2">
                    <p class="text-xs text-slate-500">End Date</p>
                    <p class="font-bold text-slate-800">
                      {{ project.endDate ? new Date(project.endDate).toLocaleDateString() : 'Infinite / Not Set' }}
                    </p>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="bg-white rounded-2xl p-6 md:p-8 shadow-sm border border-slate-100 space-y-6">
            <div class="flex items-center justify-between border-b border-slate-100 pb-4">
              <div>
                <h2 class="text-xl font-bold text-slate-900">Project Documents</h2>
                <p class="text-xs text-slate-400 mt-0.5">Manage and download technical attachments</p>
              </div>
              
              <div>
                <input 
                  type="file" 
                  ref="fileInput" 
                  @change="handleFileUpload" 
                  class="hidden" 
                />
                <button 
                  @click="fileInput?.click()" 
                  :disabled="isUploading"
                  class="px-4 py-2 bg-slate-900 hover:bg-slate-800 text-white text-xs font-bold rounded-xl transition-colors cursor-pointer flex items-center gap-1.5 disabled:opacity-50"
                >
                  <span>{{ isUploading ? 'Uploading...' : '📁 Add File' }}</span>
                </button>
              </div>
            </div>

            <div v-if="projectDocumentsList.length > 0" class="divide-y divide-slate-100 border border-slate-100 rounded-xl overflow-hidden bg-white">
                <div 
                  v-for="doc in projectDocumentsList" 
                  :key="doc.id" 
                  class="px-4 py-3.5 flex justify-between items-center hover:bg-slate-50/50 transition-all"
                >
                <div class="flex items-center gap-3 min-w-0">
                  <span class="text-2xl">📄</span>
                  <div class="min-w-0">
                    <p class="text-sm font-bold text-slate-800 truncate max-w-xs md:max-w-md">{{ doc.fileName }}</p>
                    <p class="text-xs text-slate-400 font-mono mt-0.5">Uploaded on {{ new Date().toLocaleDateString() }}</p>
                  </div>
                </div>
                <button 
                  @click="downloadDocument(doc.id, doc.fileName)"
                  class="text-xs text-emerald-600 hover:text-emerald-500 font-bold hover:underline cursor-pointer flex items-center gap-1"
                >
                  📥 Download
                </button>
              </div>
            </div>

            <div v-else class="text-center py-8 bg-slate-50 rounded-xl border border-dashed border-slate-200">
              <p class="text-sm text-slate-400">No documents uploaded yet for this project.</p>
            </div>
          </div>

          </div>

        <div class="space-y-6">
          <div class="bg-white rounded-2xl p-6 shadow-sm border border-slate-100 space-y-6">
            <h2 class="text-xl font-bold text-slate-900 border-b border-slate-100 pb-4">Project Team</h2>

            <div class="space-y-3">
              <h3 class="text-xs font-bold text-slate-400 uppercase tracking-wider">Project Manager</h3>
              <div v-if="project.projectManager" class="flex items-center gap-3 p-3 bg-slate-50 rounded-xl border border-slate-100">
                <span class="text-2xl">👑</span>
                <div>
                  <p class="text-sm font-bold text-slate-800">{{ project.projectManager.fullName }}</p>
                  <p class="text-xs text-slate-400">{{ project.projectManager.email }}</p>
                </div>
              </div>
              <div v-else class="text-sm text-amber-600 bg-amber-50 p-3 rounded-xl border border-amber-200">
                ⚠️ No manager is assigned to the project!
              </div>
            </div>

            <div class="space-y-3">
              <h3 class="text-xs font-bold text-slate-400 uppercase tracking-wider">Executors ({{ projectEmployeesList.length }})</h3>
                <div v-if="projectEmployeesList.length > 0" class="space-y-2">
                  <div 
                    v-for="emp in projectEmployeesList" 
                    :key="emp.id" 
                    class="flex items-center gap-3 p-3 hover:bg-slate-50 rounded-xl border border-transparent hover:border-slate-100 transition-all"
                  >
                  <span class="text-xl">👤</span>
                  <div>
                    <p class="text-sm font-bold text-slate-800">{{ emp.fullName }}</p>
                    <p class="text-xs text-slate-400">{{ emp.email }}</p>
                  </div>
                </div>
              </div>
              <div v-else class="text-sm text-slate-400 text-center py-6 border-2 border-dashed border-slate-100 rounded-xl">
                No employees are assigned to this project yet.
              </div>
            </div>
          </div>
        </div>

      </div>

    </div>
  </div>
</template>