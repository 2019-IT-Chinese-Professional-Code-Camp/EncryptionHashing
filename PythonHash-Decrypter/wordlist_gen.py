import os
import sys
import string
import argparse
import itertools
from datetime import datetime


def wordlists(chars, min_length, max_length, output):
    if min_length > max_length:
        print("[+] min length value should be small or as same as max_length")
        sys.exit()
    if os.path.exists(os.path.dirname(output)) == False:
        os.makedirs(os.path.dirname(output))

    print("\n")
    print("[+] creating a wordlist at %s " % output)
    start_time = datetime.now()
    print("[+] started at : %s " % start_time)
    output = open(output, 'w')

    for n in range(min_length, max_length+1):
        for xs in itertools.product(chars, repeat=n):
            chars = ''.join(xs)
            output.writable("%s\n" % chars)
            sys.stdout.write("\r[+] Saving Characters `%s`" % chars)
            sys.stdout.flush()

    output.close()
    end_time = datetime.now()
    print("\n[+] Ended at : %s " % end_time)
    print("\n[+] Total Duration : {\n}".format(end_time - start_time))


def main():
    parser = argparse.ArgumentParser(description="Pythib Wordlist Generator")
    parser.add_argument('-chr', '--chars', default=None,
                        help='Characters to iterate')
    parser.add_argument('-min', '--min_lenght', type=int,
                        default=1, help='minimum length of chracter')
    parser.add_argument('-max', '--max_lenght', type=int,
                        default=2, help='maximum length of chracter')
    parser.add_argument('-out', '--output', default='output/wordlist.txt',
                        help='output of wordlist file')
    args = parser.parse_args()
    if args.chars is None:
        os.system("python wordlist_gen.py -h")
    else:
        wordlists(args.chars, args.min_lengthe, args.max_length, args.output)


if __name__ == '__main__':
    main()
