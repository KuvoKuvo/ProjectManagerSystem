<script setup lang="ts">

defineProps<{
    files: File[]
}>()

const emit = defineEmits<{
    (e: 'file-change', event: Event): void
    (e: 'remove-file', index: number): void
}>()

</script>

<template>
  <div class="space-y-6 animate-fade-in">
    <div class="text-center max-w-md mx-auto">
      <h3 class="text-lg font-bold text-slate-800">Attach Project Documentation</h3>
      <p class="text-xs text-slate-500 mt-1">
        Upload contract agreements, requirements documents or any specifications.
      </p>
    </div>

    <div class="max-w-md mx-auto">
      <div class="border-2 border-dashed border-slate-200 hover:border-emerald-500 rounded-2xl p-6 text-center cursor-pointer transition-colors relative bg-slate-50/50">
        <input 
          type="file" 
          multiple 
          @change="emit('file-change', $event)" 
          class="absolute inset-0 opacity-0 cursor-pointer" 
        />
        <div class="space-y-1">
          <span class="text-3xl">📁</span>
          <p class="text-xs font-bold text-slate-700">Click to upload files</p>
          <p class="text-[10px] text-slate-400">PDF, Word, Excel or Images (Multiple selection is supported)</p>
        </div>
      </div>
    </div>

    <div class="max-w-md mx-auto">
      <label class="block text-xs font-semibold text-slate-500 uppercase tracking-wider mb-2">
        Files to upload ({{ files.length }})
      </label>

      <div v-if="files.length === 0" class="text-center py-6 bg-slate-50 rounded-xl border border-dashed border-slate-200">
        <p class="text-xs text-slate-400">No documents attached. You can skip this step and upload later.</p>
      </div>

      <ul v-else class="max-h-40 overflow-y-auto space-y-2 pr-1">
        <li 
          v-for="(file, index) in files" 
          :key="index"
          class="flex items-center justify-between p-2.5 bg-slate-50 border border-slate-100 rounded-xl"
        >
          <div class="flex items-center gap-2 max-w-[70%]">
            <span class="text-sm">📄</span>
            <span class="text-xs font-bold text-slate-700 truncate block">{{ file.name }}</span>
          </div>
          <button 
            type="button"
            @click="emit('remove-file', index)"
            class="text-xs text-rose-500 hover:text-rose-700 font-bold px-2 py-1 hover:bg-rose-50 rounded-lg transition-colors cursor-pointer"
          >
            Delete
          </button>
        </li>
      </ul>
    </div>
  </div>
</template>