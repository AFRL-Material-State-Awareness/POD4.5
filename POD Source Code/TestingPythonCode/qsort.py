

def qsort(mylist):
    """Quicksort using list comprehensions"""
    if mylist == []:
        return []
    else:
        pivot = mylist[0]
        lesser = qsort([x for x in mylist[1:] if x < pivot])
        greater = qsort([x for x in mylist[1:] if x >= pivot])
        mylist = lesser + [pivot] + greater
        return mylist
