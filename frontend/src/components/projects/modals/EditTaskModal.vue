<script setup lang="ts">
import type { TaskUpdatePayload } from '@/api/types'

const props = defineProps<{
  editTaskForm: TaskUpdatePayload;
  isSaving: boolean;
}>()

const emit = defineEmits<{
  (e: 'update:editTaskForm', value: TaskUpdatePayload): void
  (e: 'close'): void
  (e: 'save'): void
}>()

const updateField = (field: keyof TaskUpdatePayload, value: any) => {
  emit('update:editTaskForm', { ...props.editTaskForm, [field]: value })
}
</script>

<template>
  <div class="fixed inset-0 bg-slate-900/40 backdrop-blur-sm z-50 flex items-center justify-center p-4">
    <div class="bg-white rounded-2xl max-w-md w-full shadow-2xl overflow-hidden flex flex-col max-h-[90vh]">
      
      <div class="p-5 border-b border-slate-100 flex justify-between items-center bg-slate-50/50">
        <h3 class="text-lg font-bold text-slate-800">Edit Task</h3>
        <button @click="$emit('close')" class="text-slate-400 hover:text-slate-600 transition-colors p-1 cursor-pointer">
          ✖
        </button>
      </div>

      <div class="p-6 overflow-y-auto space-y-5">
        <div class="space-y-1.5">
          <label class="text-xs font-bold text-slate-500 uppercase tracking-wider">Task Name *</label>
          <input 
            :value="editTaskForm.name"
            @input="updateField('name', ($event.target as HTMLInputElement).value)"
            type="text" 
            class="w-full border border-slate-200 rounded-xl px-4 py-2.5 text-sm focus:ring-2 focus:ring-slate-900 focus:border-slate-900 outline-none transition-all font-medium"
          />
        </div>

        <div class="space-y-1.5">
          <label class="text-xs font-bold text-slate-500 uppercase tracking-wider">Description / Comment</label>
          <textarea 
            :value="editTaskForm.comment"
            @input="updateField('comment', ($event.target as HTMLTextAreaElement).value)"
            rows="3"
            class="w-full border border-slate-200 rounded-xl px-4 py-2.5 text-sm focus:ring-2 focus:ring-slate-900 outline-none transition-all font-medium resize-none"
          ></textarea>
        </div>

        <div class="space-y-1.5">
          <label class="text-xs font-bold text-slate-500 uppercase tracking-wider">Priority</label>
          <select 
            :value="editTaskForm.priority"
            @change="updateField('priority', Number(($event.target as HTMLSelectElement).value))"
            class="w-full border border-slate-200 rounded-xl px-4 py-2.5 text-sm focus:ring-2 focus:ring-slate-900 outline-none font-medium bg-white cursor-pointer"
          >
            <option :value="1">Low</option>
            <option :value="2">Normal</option>
            <option :value="3">High</option>
            <option :value="4">Critical</option>
          </select>
        </div>
      </div>

      <div class="p-5 border-t border-slate-100 bg-slate-50/50 flex justify-end gap-3">
        <button 
          @click="$emit('close')"
          class="px-4 py-2 text-sm font-bold text-slate-600 hover:bg-slate-200 bg-slate-100 rounded-xl transition-colors cursor-pointer"
        >
          Cancel
        </button>
        <button 
          @click="$emit('save')"
          :disabled="isSaving || !editTaskForm.name"
          class="px-4 py-2 text-sm font-bold text-white bg-slate-900 hover:bg-slate-800 rounded-xl transition-colors disabled:opacity-50 disabled:cursor-not-allowed cursor-pointer"
        >
          {{ isSaving ? 'Saving...' : 'Save Changes' }}
        </button>
      </div>
    </div>
  </div>
</template>