import os
import sys

BANNED_FUNCTIONS = ["FindObjectOfType", "FindObjectsOfType"]
directory = "."

# Folders we explicitly want the scanner to ignore
EXCLUDE_DIRS = ["Library", "Packages", "Logs", "Temp", "obj", ".git", ".vs"]

found_errors = False

print("Starting custom DevSecOps C# scan...")

for root, dirs, files in os.walk(directory):
    dirs[:] = [d for d in dirs if d not in EXCLUDE_DIRS]
    
    for file in files:
        if file.endswith(".cs"):
            filepath = os.path.join(root, file)
            with open(filepath, 'r', encoding='utf-8') as f:
                lines = f.readlines()
                for line_num, line in enumerate(lines, 1):
                    
                    # --- THE FIX ---
                    # Split the line at '//' and only scan the first part (the code)
                    # This completely ignores your documentation comments.
                    code_only = line.split('//')[0]
                    
                    for banned in BANNED_FUNCTIONS:
                        if banned in code_only:
                            print(f"::error file={filepath},line={line_num}::Deprecated function '{banned}' is not allowed. Please use FindFirstObjectByType.")
                            found_errors = True

if found_errors:
    print("Pipeline Failed: Deprecated code detected.")
    sys.exit(1)
else:
    print("Success: All code passed the quality gate.")
    sys.exit(0)
