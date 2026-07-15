import { ref, reactive, computed, onMounted } from 'vue'
import { EmployeesService } from '@/api/employees.service'
import type { Employee, EmployeeCreatePayload } from '@/api/types'

export function useEmployees() {
    const isLoading = ref(false)
    const listLoading = ref(false)
    const globalError = ref('')
    const successMessage = ref('')

    const employees = ref<Employee[]>([])
    const searchQuery = ref('')

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
    const fetchEmployees = async () => {
        try{
            listLoading.value = true
            globalError.value = ''
            employees.value = await EmployeesService.getAll()
        }
        catch (err: any){
            console.error('Failed to load employees list:', err)
            globalError.value = 'Could not load employees. Please refresh.'
        }
        finally{
            listLoading.value = false
        }
    }

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
            globalError.value = err.response?.data?.message || 'Action failed. Check fields or try again.'
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
        generatedPassword,
        isCopied,
        isEditMode,
        form,
        filteredEmployees,
        copyPassword,
        startEdit,
        cancelEdit,
        submitForm,
        deleteEmployee
    }
}