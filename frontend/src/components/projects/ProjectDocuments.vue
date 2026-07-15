<script setup lang="ts">
import type { ProjectDocument } from '@/api/types'
import { ref } from 'vue'

defineProps<{
  documents: ProjectDocument[];
  isUploading: boolean;
  isManagerOrDirector: boolean;
}>()

const emit = defineEmits<{
  (e: 'upload', event: Event): void
  (e: 'download', documentId: number, fileName: string): void
}>()

const fileInput = ref<HTMLInputElement | null>(null)

const triggerFileUpload = () => {
  fileInput.value?.click()
}
</script>

<template>
  <div class="bg-white rounded-2xl p-6 md:p-8 shadow-sm border border-slate-100 space-y-6">
    <div class="flex items-center justify-between border-b border-slate-100 pb-4">
      <div>
        <h2 class="text-xl font-bold text-slate-900">Project Documents</h2>
        <p class="text-xs text-slate-400 mt-0.5">Manage and download technical attachments</p>
      </div>
      
      <div v-if="isManagerOrDirector">
        <input 
          type="file" 
          ref="fileInput" 
          @change="(e) => emit('upload', e)" 
          class="hidden" 
        />
        <button 
          @click="triggerFileUpload" 
          :disabled="isUploading"
          class="px-4 py-2 bg-slate-900 hover:bg-slate-800 text-white text-xs font-bold rounded-xl transition-colors cursor-pointer flex items-center gap-1.5 disabled:opacity-50"
        >
          <span>{{ isUploading ? 'Uploading...' : '📁 Add File' }}</span>
        </button>
      </div>
    </div>

    <div v-if="documents.length > 0" class="divide-y divide-slate-100 border border-slate-100 rounded-xl overflow-hidden bg-white">
      <div 
        v-for="doc in documents" 
        :key="doc.id" 
        class="px-4 py-3.5 flex justify-between items-center hover:bg-slate-50/50 transition-all"
      >
        <div class="flex items-center gap-3 min-w-0">
          <span class="text-2xl">📄</span>
          <div class="min-w-0">
            <p class="text-sm font-bold text-slate-800 truncate max-w-xs md:max-w-md">{{ doc.fileName }}</p>
            <p class="text-xs text-slate-400 font-mono mt-0.5">Attachment</p>
          </div>
        </div>
        <button 
          @click="emit('download', doc.id, doc.fileName)"
          class="text-xs text-emerald-600 hover:text-emerald-500 font-bold hover:underline cursor-pointer flex items-center gap-1"
        >
          Download
        </button>
      </div>
    </div>

    <div v-else class="text-center py-8 bg-slate-50 rounded-xl border border-dashed border-slate-200">
      <p class="text-sm text-slate-400">No documents uploaded yet for this project.</p>
    </div>
  </div>
</template>