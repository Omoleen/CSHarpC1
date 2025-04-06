// Teacher AJAX operations
const teacherApi = {
    // Get all teachers
    getAll: async function() {
        try {
            const response = await fetch('/api/TeacherAPI');
            if (!response.ok) throw new Error('Failed to fetch teachers');
            return await response.json();
        } catch (error) {
            console.error('Error fetching teachers:', error);
            throw error;
        }
    },
    
    // Get teacher by ID
    getById: async function(id) {
        try {
            const response = await fetch(`/api/TeacherAPI/${id}`);
            if (!response.ok) throw new Error(`Failed to fetch teacher with ID ${id}`);
            return await response.json();
        } catch (error) {
            console.error(`Error fetching teacher ${id}:`, error);
            throw error;
        }
    },
    
    // Add a new teacher
    add: async function(teacher) {
        try {
            const response = await fetch('/api/TeacherAPI', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(teacher)
            });
            
            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || 'Failed to add teacher');
            }
            
            return await response.json();
        } catch (error) {
            console.error('Error adding teacher:', error);
            throw error;
        }
    },
    
    // Delete a teacher
    delete: async function(id) {
        try {
            const response = await fetch(`/api/TeacherAPI/${id}`, {
                method: 'DELETE'
            });
            
            if (!response.ok) {
                if (response.status === 404) {
                    throw new Error(`Teacher with ID ${id} not found`);
                }
                throw new Error('Failed to delete teacher');
            }
            
            return true; // Successfully deleted
        } catch (error) {
            console.error(`Error deleting teacher ${id}:`, error);
            throw error;
        }
    }
}; 