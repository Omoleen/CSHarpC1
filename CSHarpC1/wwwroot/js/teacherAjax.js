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
    },
    
    // Update an existing teacher
    update: async function(id, teacher) {
        try {
            const response = await fetch(`/api/TeacherAPI/${id}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(teacher)
            });

            if (!response.ok) {
                // Try to parse error details from the response body
                let errorData;
                try {
                     errorData = await response.json();
                } catch (parseError) {
                     // If parsing fails, use a generic message based on status
                     if (response.status === 404) throw new Error(`Teacher with ID ${id} not found.`);
                     if (response.status === 400) throw new Error('Invalid data submitted. Please check errors.');
                     throw new Error(`Failed to update teacher. Status: ${response.status}`);
                }
                // Use the message from the API response if available
                throw new Error(errorData.message || 'Failed to update teacher');
            }

            // PUT requests often return 204 No Content, so no body to parse on success
            return true; // Indicate success
        } catch (error) {
            console.error(`Error updating teacher ${id}:`, error);
            throw error; // Re-throw to be caught by the calling function
        }
    }
}; 