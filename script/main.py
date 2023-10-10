import numpy as np
import math
import matplotlib.pyplot as plt
import sys
import sqlite3
import os


# массивы для таблицы задание 1 и тестовое
xi = []
vi = []
v2i = []
cntrl = []
olp = []
hi = []
C1 = 0
C2 = 0
C1i = []
C2i = []
ui = []
diff = []
# + массивы для 2-го задания
u1 = []
u2 = []

A = 0
B = 1
C = 1
Nmax = 100  # количество итераций
eps = 0.001  # погрешность
e = 0.0001  # погрешность выхода на границу
p = 4  # порядок метода
hmin = 10 ** -5  # минимальный шаг(для метода с контролем погрешности)
vMax = 10  # оценка для значения v( для адекватного предстваления функции решения)
b = 2

x0 = 0
v0 = 3
h = 0.01

WC = True
taskType = "test"


def catchParamsFromCmd():
    global x0, v0, h, Nmax, eps, e, WC, A, B, C, taskType, b

    if len(sys.argv) == 1:
        print("программа запущена со стандартными параметрами")
        return

    x0 = float(sys.argv[1])
    v0 = float(sys.argv[2])
    h = float(sys.argv[3])
    Nmax = int(sys.argv[4])
    eps = float(sys.argv[5])
    e = float(sys.argv[6])
    WC = bool(int(sys.argv[7]))
    A = float(sys.argv[8])
    B = float(sys.argv[9])
    C = float(sys.argv[10])
    taskType = sys.argv[11]
    b = float(sys.argv[12])


catchParamsFromCmd()

print(x0, v0, h, Nmax, eps, e, WC, A, B, C, taskType, b)
input()

def testFunc(x=0, v=0):
    return 2 * v


# Для задачи №1
def fX(x=0, v=0):
    return (x ** 3 + 1) / (x ** 5 + 1)


def fTask1(x=0, v=0):
    a = (v ** 3) * math.sin(10 * x)
    res = fX(x) * (v ** 2) + v - a
    return res


# Решение тестовой задачи
# def decision(x=0, v=0):
#    return np.exp(2 * x)

# Решение тестовой задачи
def Const(x0=0, v0=0):
    return v0 / np.exp(2 * x0)


def decision(x=0, C=1):
    return (np.exp(2 * x) * C)


def methodStep(xn, vn, hn, f, withcontrol=False):
    k1 = f(xn, vn)
    k2 = f(xn + hn / 2, vn + hn / 2 * k1)
    k3 = f(xn + hn / 2, vn + hn / 2 * k2)
    k4 = f(xn + hn, vn + hn * k3)
    x = xn + hn
    v = vn + hn / 6 * (k1 + 2 * k2 + 2 * k3 + k4)
    if not withcontrol:
        xi.append(x)
        vi.append(v)
        hi.append(hn)
        ui.append(decision(xn, Const(x0, v0)))
        diff.append(decision(xn, Const(x0, v0))-vn)
    return x, v


def stepWithControl(x, v, h, f, eps):
    global C1, C2
    xn, vn = methodStep(x, v, h, f, True)
    xHalf, vHalf = methodStep(x, v, h / 2, f, True)
    xNext, vNext = methodStep(xHalf, vHalf, h / 2, f, True)

    S = (vNext - vn) / (2 ** p - 1)

    oldC1 = C1
    oldC2 = C2

    if abs(S) >= eps / 2 ** (p + 1) and abs(S) <= eps:
        olp.append(S)
        xi.append(xn)
        vi.append(vn)
        hi.append(h)
        v2i.append(vNext)
        cntrl.append(vNext - vn)
        C1i.append(C1 - oldC1)
        C2i.append(C2 - oldC2)
        ui.append(decision(xn, Const(x0, v0)))
        diff.append(decision(xn, Const(x0, v0))-vn)
        return xn, vn, h
    elif abs(S) < eps / 2 ** (p + 1):
        olp.append(S)
        xi.append(xn)
        vi.append(vn)
        hi.append(h)
        v2i.append(vNext)
        cntrl.append(vNext - vn)
        C1 += 1
        C1i.append(C1 - oldC1)
        C2i.append(C2 - oldC2)
        ui.append(decision(xn, Const(x0, v0)))
        diff.append(decision(xn, Const(x0, v0))-vn)
        return xn, vn, h * 2
    else:
        while abs(S) > eps:
            C2 += 1
            h /= 2
            xn, vn = methodStep(x, v, h, f, True)
            xHalf, vHalf = methodStep(x, v, h / 2, f, True)
            xNext, vNext = methodStep(xHalf, vHalf, h / 2, f, True)
            S = (vNext - vn) / (2 ** p - 1)
        olp.append(S)
        xi.append(xn)
        vi.append(vn)
        hi.append(h)
        v2i.append(vNext)
        cntrl.append(vNext - vn)
        C1i.append(C1 - oldC1)
        C2i.append(C2 - oldC2)
        ui.append(decision(xn, Const(x0, v0)))
        diff.append(decision(xn, Const(x0, v0))-vn)
        return xn, vn, h


def RK4WC(x, v, h, Nmax, b, e, f, eps):
    xArr = []
    vArr = []

    xArr.append(x)
    vArr.append(v)

    for i in range(1, Nmax + 1):
        x, v, h = stepWithControl(x, v, h, f, eps)
        if v > vMax:
            xArr.append(x)
            vArr.append(v)
            return xArr, vArr
        if h < hmin:
            xArr.append(x)
            vArr.append(v)
            return xArr, vArr
        if x >= b - e and x <= b:
            xArr.append(x)
            vArr.append(v)
            return xArr, vArr
        if x > b:
            x, v = methodStep(xArr[i - 1], vArr[i - 1], b - xArr[i - 1], f, True)
            xArr.append(x)
            vArr.append(v)
            return xArr, vArr
        xArr.append(x)
        vArr.append(v)
    return xArr, vArr


def RK4(x, v, h, Nmax, b, e, f):
    xArr = []
    vArr = []

    xArr.append(x)
    vArr.append(v)

    for i in range(1, Nmax + 1):
        x, v = methodStep(x, v, h, f)
        if v > vMax:
            xArr.append(x)
            vArr.append(v)
            return xArr, vArr
        if x >= b - e and x <= b:
            xArr.append(x)
            vArr.append(v)
            return xArr, vArr
        if x > b:
            x, v = methodStep(xArr[i - 1], vArr[i - 1], b - xArr[i - 1], f)
            xArr.append(x)
            vArr.append(v)
            return xArr, vArr
        xArr.append(x)
        vArr.append(v)
    return xArr, vArr


# Задача №2
def func1(u1=0, u2=0):
    return u2


def func2(u1=0, u2=0, x=0):
    a, b, c = initParams()
    return -a * abs(u2) * u2 + b * u2 + c * u1


# Для задачи параметров
def initParams():
    return A, B, C


def stepForSystem(x, v1, v2, h, f1, f2):
    k11 = f1(x, v1)
    k21 = f1(x + h / 2, v1 + h / 2 * k11)
    k31 = f1(x + h / 2, v1 + h / 2 * k21)
    k41 = f1(x + h, v1 + h * k31)
    k12 = f2(x, v2)
    k22 = f2(x + h / 2, v2 + h / 2 * k12)
    k32 = f2(x + h / 2, v2 + h / 2 * k22)
    k42 = f2(x + h, v2 + h * k32)
    xn = x + h
    vn1 = v1 + h / 6 * (k11 + 2 * k21 + 2 * k31 + k41)
    vn2 = v2 + h / 6 * (k12 + 2 * k22 + 2 * k32 + k42)
    xi.append(xn)
    u1.append(vn1)
    u2.append(vn2)
    return x, vn1, vn2


def stepForSystemWithControl(x, v1, v2, h, f1, f2, eps):
    xn, vn1, vn2 = stepForSystem(x, v1, v2, h, f1, f2)
    xHalf, v1Half, v2Half = stepForSystem(x, v1, v2, h, f1, f2)
    xNext, v1Next, v2Next = stepForSystem(x, v1Half, v2Half, h, f1, f2)

    S1 = (v1Next - vn1) / (2 ** p - 1)
    S2 = (v2Next - vn2) / (2 ** p - 1)

    S = max(abs(S1), abs(S2))

    if abs(S) >= eps / 2 ** (p + 1) and abs(S) <= eps:
        olp.append(S1)
        xi.append(xn)
        u1.append(vn1)
        u2.append(vn2)
        hi.append(h)
        v2i.append(v1Next)
        cntrl.append(v1Next - vn1)
        return xn, vn1, vn2, h
    elif abs(S) < eps / 2 ** (p + 1):
        olp.append(S1)
        u1.append(vn1)
        u2.append(vn2)
        hi.append(h)
        v2i.append(v1Next)
        cntrl.append(v1Next - vn1)
        global C1
        C1 += 1
        return xn, vn1, vn2, h * 2
    else:
        while abs(S) > eps:
            global C2
            C2 += 1
            h /= 2
            xn, vn1, vn2 = stepForSystem(x, v1, v2, h, f1, f2)
            xHalf, v1Half, v2Half = stepForSystem(x, v1, v2, h, f1, f2)
            xNext, v1Next, v2Next = stepForSystem(x, v1Half, v2Half, h, f1, f2)
            S1 = (v1Next - vn1) / (2 ** p - 1)
            S2 = (v2Next - vn2) / (2 ** p - 1)
            S = max(abs(S1), abs(S2))
        olp.append(S1)
        xi.append(xn)
        u1.append(vn1)
        u2.append(vn2)
        hi.append(h)
        v2i.append(v1Next)
        cntrl.append(v1Next - vn1)
        return xn, vn1, vn2, h


def RK4Sys(x, v1, v2, h, Nmax, b, e, f1, f2):
    xArr = []
    v1Arr = []
    v2Arr = []

    xArr.append(x)
    v1Arr.append(v1)
    v2Arr.append(v2)

    for i in range(1, Nmax + 1):
        x, v1, v2 = stepForSystem(x, v1, v2, h, f1, f2)
        if v1 > vMax:
            xArr.append(x)
            v1Arr.append(v1)
            v2Arr.append(v2)
            return xArr, v1Arr, v2Arr
        if x >= b - e and x <= b:
            xArr.append(x)
            v1Arr.append(v1)
            v2Arr.append(v2)
            return xArr, v1Arr, v2Arr
        if x > b:
            x, v1, v2 = stepForSystem(xArr[i - 1], v1Arr[i - 1], v2Arr[i - 1], b - xArr[i - 1], f1, f2)
            xArr.append(x)
            v1Arr.append(v1)
            v2Arr.append(v2)
            return xArr, v1Arr, v2Arr
        xArr.append(x)
        v1Arr.append(v1)
        v2Arr.append(v2)
    return xArr, v1Arr, v2Arr


def RK4WCSys(x, v1, v2, h, Nmax, b, e, f1, f2, eps):
    xArr = []
    v1Arr = []
    v2Arr = []

    xArr.append(x)
    v1Arr.append(v1)
    v2Arr.append(v2)

    for i in range(1, Nmax + 1):
        x, v1, v2 = stepForSystemWithControl(x, v1, v2, h, f1, f2, eps)
        if v1 > vMax:
            xArr.append(x)
            v1Arr.append(v1)
            v2Arr.append(v2)
            return xArr, v1Arr, v2Arr
        if h < hmin:
            xArr.append(x)
            v1Arr.append(v1)
            v2Arr.append(v2)
            return xArr, v1Arr, v2Arr
        if x >= b - e and x <= b:
            xArr.append(x)
            v1Arr.append(v1)
            v2Arr.append(v2)
            return xArr, v1Arr, v2Arr
        if x > b:
            x, v1, v2 = stepForSystem(xArr[i - 1], v1Arr[i - 1], v2Arr[i - 1], b - xArr[i - 1], f1, f2)
            xArr.append(x)
            v1Arr.append(v1)
            v2Arr.append(v2)
            return xArr, v1Arr, v2Arr
        xArr.append(x)
        v1Arr.append(v1)
        v2Arr.append(v2)
    return xArr, v1Arr, v2Arr


if taskType == "test":
    if WC:
        RK4WC(x0, v0, h, Nmax, b, e, fTask1, eps)
    else:
        RK4(x0, v0, h, Nmax, b, e, fTask1)
elif taskType == "main1":
    if WC:
        RK4WC(x0, v0, h, Nmax, b, e, fX, eps)
    else:
        RK4(x0, v0, h, Nmax, b, e, fX)
else:
    if WC:
        pass
    else:
        pass


print("size: " , len(vi), len(v2i))

connection = sqlite3.connect("../database/lab1.sqlite3")
cursor = connection.cursor()

if taskType == "test":
    cursor.execute("delete from test where x0=? and u0=?", [x0, v0])
    for i in range(len(xi)):
        if WC:
            cursor.executemany("insert into test values(?,?,?,?,?,?,?,?,?,?,?,?,?)",
                           [[x0, v0, i + 1, xi[i], vi[i], v2i[i], cntrl[i], olp[i],
                             hi[i], C1i[i], C2i[i], ui[i], diff[i]]])
        else:
            cursor.executemany("insert into test values(?,?,?,?,?,?,?,?,?,?,?,?,?)",
                           [[x0, v0, i + 1, xi[i], vi[i], 0, 0, 0,
                             hi[i], 0, 0, ui[i], diff[i]]])

elif taskType == "main1":
    cursor.execute("delete from main1 where x0=? and u0=?", [x0, v0])
    for i in range(len(xi)):
        if WC:
            cursor.executemany("insert into main1 values(?,?,?,?,?,?,?,?,?,?,?)",
                           [[x0, v0, i + 1, xi[i], vi[i], v2i[i], cntrl[i], olp[i],
                             hi[i], C1i[i], C2i[i]]])
        else:
            cursor.executemany("insert into main1 values(?,?,?,?,?,?,?,?,?,?,?)",
                           [[x0, v0, i + 1, xi[i], vi[i], 0, 0, 0,
                             hi[i], 0, 0]])


else:
    pass

connection.commit()


# xArr, vArr = RK4WC(x0, v0, h, Nmax, b, e, fTask1, eps)
# Для тестовой задачи
# x1Arr = []
# i = 0
# while (i<=b):
#    x1Arr.append(i)
#    u1.append(decision(i))
#    diff.append(u1[i] - vi[i])
#    i += h

# plt.plot(xArr, vArr, color='red')
# plt.grid()
# plt.show()
# plt.plot(x1Arr, v1Arr, color = 'blue')
# plt.grid()
# plt.show()


print("end")
input()