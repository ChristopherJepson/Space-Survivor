import os
import sys

# The exact Unity functions we want GitHub to block
BANNED_FUNCTIONS = ["FindObjectOfType", "FindObjectsOfType"]
directory = "."

found_errors = False

print("Starting custom DevSecOps C# scan...")

# Walk through all folders and look for C# files
for root, dirs, files in os.walk(directory):
    for file in files:
        if file.endswith(".cs"):
            filepath = os.path.join(root, file)
            with open(filepath, 'r', encoding='utf-8') as f:
                lines = f.readlines()
                for line_num, line in enumerate(lines, 1):
                    for banned in BANNED_FUNCTIONS:
                        if banned in line:
                            # This specific print format tells GitHub to highlight the exact line in red!
                            print(f"::error file={filepath},line={line_num}::Deprecated function '{banned}' is not allowed. Please use FindFirstObjectByType.")
                            found_errors = True

# If we found banned code, exit with code 1 to fail the pipeline
if found_errors:
    print("Pipeline Failed: Deprecated code detected.")
    sys.exit(1)
else:
    print("Success: All code passed the quality gate.")
    sys.exit(0)