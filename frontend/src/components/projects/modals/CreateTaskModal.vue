<script setup lang="ts">

defineProps<{
  newTask: {
    name: string;
    comment: string;
    priority: number;
    status: number;
    assigneeId: number | null;
  };
  isSaving: boolean;
  employees: Array<{ id: number; fullName: string; email: string }>;
}>()

const emit = defineEmits<{
  (e: 'update:newTask', value: any): void
  (e: 'close'): void
  (e: 'create'): void
}>()

const updateField = (field: string, value: any, currentTask: any) => {
  emit('update:newTask', { ...currentTask, [field]: value })
}

</script>

<template>
  <div class="fixed inset-0 bg-slate-900/40 backdrop-blur-sm z-50 flex items-center justify-center p-4">
    <div class="bg-white rounded-2xl max-w-md w-full shadow-2xl overflow-hidden flex flex-col max-h-[90vh]">
      
      <div class="p-5 border-b border-slate-100 flex justify-between items-center bg-slate-50/50">
        <h3 class="text-lg font-bold text-slate-800">Create New Task</h3>
        <button @click="$emit('close')" class="text-slate-400 hover:text-slate-600 transition-colors p-1 cursor-pointer">
          ✖
        </button>
      </div>

      <div class="p-6 overflow-y-auto space-y-5">
        <div class="space-y-1.5">
          <label class="text-xs font-bold text-slate-500 uppercase tracking-wider">Task Name *</label>
          <input 
            :value="newTask.name"
            @input="updateField('name', ($event.target as HTMLInputElement).value, newTask)"
            type="text" 
            class="w-full border border-slate-200 rounded-xl px-4 py-2.5 text-sm focus:ring-2 focus:ring-slate-900 focus:border-slate-900 outline-none transition-all font-medium placeholder-slate-300"
          />
        </div>

        <div class="space-y-1.5">
          <label class="text-xs font-bold text-slate-500 uppercase tracking-wider">Description / Comment</label>
          <textarea 
            :value="newTask.comment"
            @input="updateField('comment', ($event.target as HTMLTextAreaElement).value, newTask)"
            rows="3"
            class="w-full border border-slate-200 rounded-xl px-4 py-2.5 text-sm focus:ring-2 focus:ring-slate-900 outline-none transition-all font-medium placeholder-slate-300 resize-none"
          ></textarea>
        </div>

        <div class="grid grid-cols-2 gap-4">
          <div class="space-y-1.5">
            <label class="text-xs font-bold text-slate-500 uppercase tracking-wider">Priority</label>
            <select 
              :value="newTask.priority"
              @change="updateField('priority', Number(($event.target as HTMLSelectElement).value), newTask)"
              class="w-full border border-slate-200 rounded-xl px-4 py-2.5 text-sm focus:ring-2 focus:ring-slate-900 outline-none font-medium bg-white cursor-pointer"
            >
              <option :value="1">Low</option>
              <option :value="2">Normal</option>
              <option :value="3">High</option>
              <option :value="4">Critical</option>
            </select>
          </div>

          <div class="space-y-1.5">
            <label class="text-xs font-bold text-slate-500 uppercase tracking-wider">Assignee</label>
            <select 
              :value="newTask.assigneeId"
              @change="updateField('assigneeId', Number(($event.target as HTMLSelectElement).value), newTask)"
              class="w-full border border-slate-200 rounded-xl px-4 py-2.5 text-sm focus:ring-2 focus:ring-slate-900 outline-none font-medium bg-white cursor-pointer"
            >
              <option :value="0">Unassigned</option>
              <option v-for="emp in employees" :key="emp.id" :value="emp.id">
                {{ emp.fullName }}
              </option>
            </select>
          </div>
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
          @click="$emit('create')"
          :disabled="isSaving || !newTask.name"
          class="px-4 py-2 text-sm font-bold text-white bg-slate-900 hover:bg-slate-800 rounded-xl transition-colors disabled:opacity-50 disabled:cursor-not-allowed cursor-pointer"
        >
          {{ isSaving ? 'Creating...' : 'Create Task' }}
        </button>
      </div>
    </div>
  </div>
</template>