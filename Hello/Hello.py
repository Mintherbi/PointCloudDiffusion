import time

print("wait for 10 second...")
for i in range(10):
    print(f"{i+1} second passed...")
    time.sleep(1)
print("Finished!")