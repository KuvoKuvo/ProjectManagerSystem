<script setup lang="ts">
import type { Task, TaskFilters } from '@/api/types'

type TaskWithDetails = Task & { assigneeFullName?: string }

const props = defineProps<{
  tasks: TaskWithDetails[];
  isManagerOrDirector: boolean;
  projectEmployeesList: Array<{ id: number; fullName: string; email: string }>;
  filters: TaskFilters;
  getPriorityLabel: (priority: number) => { text: string, class: string };
  getStatusDetails: (status: number) => { text: string, class: string };
}>()

const emit = defineEmits<{
  (e: 'create'): void
  (e: 'edit', task: TaskWithDetails): void
  (e: 'delete', taskId: number): void
  (e: 'change-assignee', taskId: number, assigneeId: number): void
  (e: 'update-status', taskId: number, status: number): void
  (e: 'update:filters', filters: TaskFilters): void
}>()

const updateFilter = (key: keyof TaskFilters, value: any) => {
  emit('update:filters', { ...props.filters, [key]: value })
}
</script>

<template>
  <div class="bg-white rounded-2xl p-6 md:p-8 shadow-sm border border-slate-100 space-y-6">
    <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4 border-b border-slate-100 pb-4">
      <div>
        <h2 class="text-xl font-bold text-slate-900">Tasks</h2>
        <p class="text-xs text-slate-400 mt-0.5">Manage workflow and track assignees</p>
      </div>
      <button 
        v-if="isManagerOrDirector"
        @click="$emit('create')"
        class="px-4 py-2 bg-slate-900 hover:bg-slate-800 text-white text-xs font-bold rounded-xl transition-colors cursor-pointer flex items-center justify-center gap-1.5"
      >
        <span>➕ Create Task</span>
      </button>
    </div>

    <div class="flex flex-wrap items-center gap-3 bg-slate-50 p-3 rounded-xl border border-slate-100">
      <div class="flex items-center gap-2">
        <span class="text-xs font-bold text-slate-500 uppercase">Status:</span>
        <select 
          :value="filters.status" 
          @change="updateFilter('status', ($event.target as HTMLSelectElement).value === '' ? '' : Number(($event.target as HTMLSelectElement).value))"
          class="text-xs border border-slate-200 rounded-lg px-2 py-1.5 outline-none bg-white font-medium"
        >
          <option value="">All Statuses</option>
          <option :value="0">To Do</option>
          <option :value="1">In Work</option>
          <option :value="2">Done</option>
        </select>
      </div>

      <button 
        @click="updateFilter('isDescending', !filters.isDescending)"
        class="text-xs border border-slate-200 bg-white rounded-lg px-2 py-1.5 font-medium hover:bg-slate-50 transition-colors"
        title="Toggle sorting direction"
      >
        {{ filters.isDescending ? '⬇️ Desc' : '⬆️ Asc' }}
      </button>
    </div>

    <div v-if="tasks.length > 0" class="space-y-4">
      <div 
        v-for="task in tasks" 
        :key="task.id" 
        class="p-5 bg-white border border-slate-200/80 rounded-2xl shadow-sm hover:shadow-md transition-all space-y-4"
      >
        <div class="flex items-start justify-between gap-4">
          <div>
            <div class="flex items-center gap-2 flex-wrap">
              <span class="inline-flex px-2 py-0.5 rounded text-xs font-semibold" :class="getPriorityLabel(task.priority).class">
                {{ getPriorityLabel(task.priority).text }}
              </span>
              <span class="inline-flex px-2 py-0.5 rounded text-xs font-semibold border" :class="getStatusDetails(task.status).class">
                {{ getStatusDetails(task.status).text }}
              </span>
            </div>
            <h4 class="text-base font-bold text-slate-800 mt-2">{{ task.name }}</h4>
            <p class="text-sm text-slate-500 mt-1" v-if="task.comment">{{ task.comment }}</p>
          </div>

          <div v-if="isManagerOrDirector" class="flex items-center gap-1">
            <button 
              @click="$emit('edit', task)"
              class="text-xs text-blue-500 hover:text-blue-700 font-bold p-1 hover:bg-blue-50 rounded transition-all cursor-pointer"
              title="Edit task"
            >
              ✏️
            </button>
            <button 
              @click="$emit('delete', task.id)"
              class="text-xs text-red-500 hover:text-red-700 font-bold p-1 hover:bg-red-50 rounded transition-all cursor-pointer"
              title="Delete task"
            >
              🗑️
            </button>
          </div>
        </div>

        <div class="border-t border-slate-100 pt-4 flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 text-xs">
          <div class="flex items-center gap-2">
            <span class="text-slate-400">Assignee:</span>
            <div v-if="isManagerOrDirector">
              <select 
                :value="task.assigneeId" 
                @change="$emit('change-assignee', task.id, Number(($event.target as HTMLSelectElement).value))"
                class="bg-slate-50 border border-slate-200 text-slate-700 text-xs rounded-lg p-1 font-semibold focus:ring-1 focus:ring-slate-300 outline-none cursor-pointer"
              >
                <option :value="0">Unassigned</option>
                <option v-for="emp in projectEmployeesList" :key="emp.id" :value="emp.id">
                  {{ emp.fullName }}
                </option>
              </select>
            </div>
            <span v-else class="font-bold text-slate-700 bg-slate-100 py-0.5 px-2 rounded-full">
              {{ task.assigneeFullName || 'Unassigned' }}
            </span>
          </div>

          <div class="flex items-center gap-2">
            <span class="text-slate-400">Status:</span>
            <div class="flex gap-1">
              <button @click="$emit('update-status', task.id, 0)" :disabled="task.status === 0" class="px-2 py-1 border rounded hover:bg-slate-50 disabled:bg-slate-100 disabled:text-slate-400 font-semibold">To Do</button>
              <button @click="$emit('update-status', task.id, 1)" :disabled="task.status === 1" class="px-2 py-1 border rounded hover:bg-slate-50 disabled:bg-indigo-500 disabled:text-white disabled:border-indigo-500 font-semibold">In Work</button>
              <button @click="$emit('update-status', task.id, 2)" :disabled="task.status === 2" class="px-2 py-1 border rounded hover:bg-slate-50 disabled:bg-emerald-500 disabled:text-white disabled:border-emerald-500 font-semibold">Done</button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div v-else class="text-center py-10 bg-slate-50 rounded-xl border border-dashed border-slate-200">
      <p class="text-sm text-slate-400">No tasks found matching your criteria.</p>
    </div>
  </div>
</template>