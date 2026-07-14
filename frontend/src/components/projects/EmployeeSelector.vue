<script setup lang="ts">

interface Employee {
    id: number
    fullName: string
    email: string
}

defineProps<{
    allEmployees: Employee[]
}>()

const selectedIds = defineModel<number[]>({
    default: () => []
})

const addEmployee = (id: number) => {
    if (!id) return
    if (!selectedIds.value.includes(id)) {
        selectedIds.value.push(id)
    }
}

const removeEmployee = (id: number) => {
  selectedIds.value = selectedIds.value.filter(empId => empId !== id)
}

</script>

<template>
  <div class="border-t border-slate-100 pt-4 space-y-3">
    <label class="block text-xs font-bold text-slate-500 uppercase">Project Executors (Team)</label>
    <div class="flex gap-2">
      <select 
        @change="addEmployee(Number(($event.target as HTMLSelectElement).value)); ($event.target as HTMLSelectElement).value = ''"
        class="w-full bg-slate-50 border border-slate-200 rounded-xl p-2.5 text-sm outline-none focus:border-slate-400"
      >
        <option value="" disabled selected>Select an employee to add...</option>
        <option 
          v-for="emp in allEmployees" 
          :key="emp.id" 
          :value="emp.id"
          :disabled="selectedIds.includes(emp.id)"
        >
          {{ emp.fullName }} ({{ emp.email }})
        </option>
      </select>
    </div>

    <div v-if="selectedIds.length > 0" class="space-y-2 max-h-48 overflow-y-auto border border-slate-100 rounded-xl p-2 bg-slate-50/50">
      <div 
        v-for="empId in selectedIds" 
        :key="empId"
        class="flex items-center justify-between bg-white px-3 py-2 rounded-lg border border-slate-200/60 text-xs"
      >
        <div class="flex items-center gap-2">
          <span class="text-sm">👤</span>
          <div>
            <p class="font-bold text-slate-800">
              {{ allEmployees.find(e => e.id === empId)?.fullName || 'Loading...' }}
            </p>
            <p class="text-[10px] text-slate-400">
              {{ allEmployees.find(e => e.id === empId)?.email }}
            </p>
          </div>
        </div>
        <button 
          type="button"
          @click="removeEmployee(empId)"
          class="text-xs text-red-500 hover:text-red-700 font-bold px-2 py-1 hover:bg-red-50 rounded-lg transition-colors cursor-pointer"
          title="Remove from project"
        >
          Remove
        </button>
      </div>
    </div>
    
    <div v-else class="text-center py-6 bg-slate-50 rounded-xl border border-dashed border-slate-200">
      <p class="text-xs text-slate-400">No executors assigned to this project yet.</p>
    </div>
  </div>
</template>