import sys
import math
import re
from itertools import islice

wally_pattern = [] 
line_picture = [] 
solution = [] 

wally_width, wally_height = [int(i) for i in input().split()]
# print(wally_width, wally_height)
for i in range(wally_height):
    wally_row = input()
    # print(wally_row)
    w_p = wally_row.replace(" ",".")
    w_p = re.sub(r"[\/\\\'\`\"\[\]\_\-\|\#\~\(\)]", r"\\\g<0>", w_p)
    wally_pattern.append(w_p)


picture_width, picture_height = [int(i) for i in input().split()]
# print(picture_width, picture_height)
for i in range(picture_height):
    picture_row = input()
    # print(picture_row)
    line_picture.append(picture_row)


for line in range(picture_height-wally_height) :
    choice=[]
    
    
    for x_choice in range(0,picture_width-wally_width+1) :
        motif_line = line_picture[line][x_choice:x_choice+wally_width]
        match = re.search(wally_pattern[0],motif_line)
        if match : 
            statut = 1
            for k in range(1,wally_height) :
                motif_line2 = line_picture[line+k][x_choice:x_choice+wally_width]
                match = re.search(wally_pattern[k],motif_line2)
                if match :
                    pass
                else :
                    statut = 0
            if statut == 1 : 
                choice.append([x_choice,line])
        else : 
            pass
    if choice : 
        solution.extend(choice)

if len(solution)==1 : 
    print(solution[0][0],solution[0][1])
else : 
    print("Bad")