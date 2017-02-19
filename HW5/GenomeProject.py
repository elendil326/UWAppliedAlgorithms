import numpy as np
import matplotlib.pyplot as plt
import plotly.plotly as py
from collections import Counter

individuals = 995
columns = 10101
X = [[0 for j in range(columns)] for i in range(individuals)]

i = 0
f = open('genome-data.txt')
for line in f:
	cols = line.strip().split()
	for j in range(3, columns + 3):
		X[i][j - 3] = cols[j]
	i = i + 1
f.close()

modes = ['A' for i in range(columns)]
for i in range(columns):
	modeDict = { "A" : 0, "C" : 0, "G" : 0, "T" : 0, "0" : 0 }
	for j in range(individuals):
		modeDict[X[j][i]] = modeDict[X[j][i]] + 1
	modeMax = float("-inf")
	for key, value in modeDict.iteritems():
		if modeMax < value:
			modes[i] = key
			modeMax = value

for i in range(individuals):
	for j in range(columns):
		if X[i][j] != modes[j]:
			X[i][j] = 1
		else:
			X[i][j] = 0

np.random.seed(0)

x, y = np.random.randn(2, 100)
fig = plt.figure()
ax1 = fig.add_subplot(211)
ax1.xcorr(x, y, usevlines=True, maxlags=50, normed=True, lw=2)
ax1.grid(True)
ax1.axhline(0, color='black', lw=2)

ax2 = fig.add_subplot(212, sharex=ax1)
ax2.acorr(x, usevlines=True, normed=True, maxlags=50, lw=2)
ax2.grid(True)
ax2.axhline(0, color='black', lw=2)

plt.show()




