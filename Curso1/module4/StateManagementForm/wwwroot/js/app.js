// save user preferences to localStorage
function savePreferences(preferences) {
    // Convert preferences object to JSON string for storage
    localStorage.setItem('userPreferences', JSON.stringify(preferences));
}

// Retrieve user preferences from local storage
function getPreferences() {
    // Get the stored JSON string
    const stored = localStorage.getItem('userPreferences');
    
    // Convert JSON string back to object (or return null if not found)
    return stored ? JSON.parse(stored) : null;
}

// Example: Saving preferences
const userPrefs = {
    theme: 'Dark',
    language: 'English',
    fontSize: '16px'
};

savePreferences(userPrefs);

// Example: Retrieving preferences
const currentPrefs = getPreferences();
if (currentPrefs) {
    console.log('Current theme:', currentPrefs.theme);
    console.log('Current language:', currentPrefs.language);
    console.log('Current fontSize: ', currentPrefs.fontSize);
}