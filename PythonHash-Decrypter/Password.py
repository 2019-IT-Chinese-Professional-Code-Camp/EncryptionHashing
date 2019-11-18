import time
import hashlib

counter = 1
md5_pass = input("Please enter md5 passord : ")
md5_file = input("Please enter word list location: ")

try:
    md5_file = open(md5_file, 'r')
except:
    print("\nFile NOT Found")
    quit()

for password in md5_file:
    hash_obj = hashlib.md5(password.strip().encode('utf-8')).hexdigest()
    start = time.time()
    print("Trying password %d -------> %s " % (counter, password.strip()))
    counter += 1
    end = time.time()
    t_time = end - start

    if hash_obj == md5_pass:
        print("Password found !!!! password is : %s" % password)
        print("Total runing time : ", t_time, "seconds")
        time.sleep(10)
        break
else:
    print("Password not found")
