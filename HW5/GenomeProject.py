import numpy as np
import matplotlib.pyplot as plt
import plotly.plotly as py
from collections import Counter
from sklearn.decomposition import PCA

# Initialize values
individuals = 995
columns = 10101
fullData = [[0 for j in range(columns + 3)] for i in range(individuals)] 
X = [[0 for j in range(columns)] for i in range(individuals)]

# Read file
i = 0
f = open('genome-data.txt')
for line in f:
	cols = line.strip().split()
	for j in range(columns + 3):
		fullData[i][j] = cols[j]
	i = i + 1
f.close()

modes = ['A' for i in range(columns)]
for i in range(columns):
	modeDict = { "A" : 0, "C" : 0, "G" : 0, "T" : 0, "0" : 0 }
	for j in range(individuals):
		modeDict[fullData[j][i + 3]] = modeDict[fullData[j][i + 3]] + 1
	modeMax = float("-inf")
	for key, value in modeDict.iteritems():
		if modeMax < value:
			modes[i] = key
			modeMax = value

for i in range(individuals):
	for j in range(columns):
		if fullData[i][j + 3] != modes[j]:
			X[i][j] = 1
		else:
			X[i][j] = 0

# Section (a)

pca = PCA(n_components=2)
pca.fit(X)

N = 50
x = np.random.rand(N)
y = np.random.rand(N)
colors = np.random.rand(N)
area = np.pi * (15 * np.random.rand(N))**2  # 0 to 15 point radii

plt.scatter(x, y, s=area, c=colors, alpha=0.5)
plt.show()




