import { ref, reactive, computed, watch, onMounted } from 'vue'
import { EmployeesService } from '@/api/employees.service'
import type { Employee, EmployeeCreatePayload } from '@/api/types'

function parseApiError(err: any, fallbackMessage: string = 'An error occurred'): string {
  if (err?.response?.data) {
    const data = err.response.data
    
    if (data.message) return data.message
    if (data.errors && typeof data.errors === 'object') {
      const errorMessages: string[] = []
      for (const key of Object.keys(data.errors)) {
        if (Array.isArray(data.errors[key])) {
          errorMessages.push(...data.errors[key])
        }
      }
      if (errorMessages.length > 0) {
        return errorMessages.join(', ')
      }
    }
  }
  return fallbackMessage
}

export function useEmployees() {
    const isLoading = ref(false)
    const listLoading = ref(false)
    const globalError = ref('')
    const successMessage = ref('')

    const employees = ref<Employee[]>([])
    const searchQuery = ref('')
    let searchTimeout: any = null

    const generatedPassword = ref('')
    const isCopied = ref(false)

    const isEditMode = ref(false)
    const currEditingId = ref<number | null>(null)

    const initialFormState: EmployeeCreatePayload = {
        firstName: '',
        lastName: '',
        middleName: '',
        email: '',
        role: 'Employee'
    }

    const form = reactive<EmployeeCreatePayload>({ ...initialFormState })

    // Loading employee list
    const fetchEmployees = async (query: string = '') => {
        try{
            listLoading.value = true
            if (query.trim()){
                employees.value = await EmployeesService.search(query.trim())
            }
            else{
                employees.value = await EmployeesService.getAll()
            }
        }
        catch (err: any){
            console.error('Failed to fetch employees:', err)
        }
        finally{
            listLoading.value = false
        }
    }

    watch(searchQuery, (newQuery) => {
        clearTimeout(searchTimeout)
        searchTimeout = setTimeout(() => {
            fetchEmployees(newQuery)
        }, 300)
    })

    // Client-side filtering (computed property)
    const filteredEmployees = computed(() => {
        const query = searchQuery.value.toLowerCase().trim()
        if(!query) return employees.value

        return employees.value.filter((emp) => {
            const fullName = `${emp.firstName} ${emp.lastName} ${emp.middleName || ''}`.toLowerCase()

            return fullName.includes(query) ||
               emp.email.toLowerCase().includes(query) ||
               emp.role.toLowerCase().includes(query)
        })
    })

    // Copying the password to the clipboard
    const copyPassword = async () => {
        if (!generatedPassword.value) return
        try{
            await navigator.clipboard.writeText(generatedPassword.value)
            isCopied.value = true
            setTimeout(() =>{
                isCopied.value = false
            }, 2000)
        }
        catch (err){
            console.error('Failed to copy password:', err)
        }
    }

    // Filling out the editing form
    const startEdit = (emp: Employee) => {
        isEditMode.value = true
        currEditingId.value = emp.id
        generatedPassword.value = ''

        form.firstName = emp.firstName
        form.lastName = emp.lastName
        form.middleName = emp.middleName || ''
        form.email = emp.email
        form.role = emp.role

        globalError.value = ''
        successMessage.value = ''
    }

    const cancelEdit = () => {
        isEditMode.value = false
        currEditingId.value = null
        resetForm()
    }

    const resetForm = () => {
        Object.assign(form, initialFormState)
    }

    // Form Submission (Create or Update)
    const submitForm = async () => {
        try{
            isLoading.value = true
            globalError.value = ''
            successMessage.value = ''

            if(isEditMode.value && currEditingId.value !== null){
                // Editing
                await EmployeesService.update(currEditingId.value, {...form})
                successMessage.value = `Employee ${form.firstName} ${form.lastName} updated successfully!`
                cancelEdit()
            }
            else{
                // Creating a new employee
                const result = await EmployeesService.create({...form})
                if(result.temporaryPassword){
                    generatedPassword.value = result.temporaryPassword
                }
                successMessage.value = `Account for ${form.firstName} has been successfully created!`
                resetForm()
            }
            await fetchEmployees()
        }
        catch (err: any){
            console.error('Save employee error:', err)
            globalError.value = parseApiError(err, 'Failed to save employee data.')
        }
        finally{
            isLoading.value = false
        }
    }
    
    // Deleting an employee
    const deleteEmployee = async (id: number, name: string) => {
        if (!confirm(`Are you sure you want to delete employee "${name}"?`)){
            return
        }
        try{
            listLoading.value = true
            globalError.value = ''
            await EmployeesService.delete(id)

            if (isEditMode.value && currEditingId.value === id) {
                cancelEdit()
            }
            await fetchEmployees()
        }
        catch (err: any){
            console.error('Failed to delete employee:', err)
            globalError.value = err.response?.data?.message || 'Could not delete employee.'
        }
        finally{
            listLoading.value = false
        }
    }
    onMounted(() => {
        fetchEmployees()
    })

    return {
        isLoading,
        listLoading,
        globalError,
        successMessage,
        searchQuery,
        employees,
        filteredEmployees: employees,
        generatedPassword,
        isCopied,
        isEditMode,
        form,
        copyPassword,
        startEdit,
        cancelEdit,
        submitForm,
        deleteEmployee
  }
}