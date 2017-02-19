import numpy as np
import matplotlib.pyplot as plt
import plotly.plotly as py
from collections import Counter
from sklearn.decomposition import PCA
from array import array

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
reducedX = pca.fit_transform(X)

# Initialize colors per population
colorsPerPopulation = dict()
colors = [0 for i in range(individuals)]
for i in range(individuals):
	if fullData[i][2] not in colorsPerPopulation:
		colorsPerPopulation[fullData[i][2]] = np.random.random()
	colors[i] = colorsPerPopulation[fullData[i][2]]

scatterPlotsPerPopulation = [0 for i in range(len(colorsPerPopulation))]
legendsPerPopulation = [0 for i in range(len(colorsPerPopulation))]
j = 0
for key, value in colorsPerPopulation.iteritems():
	x = []
	y = []
	for i in range(individuals):
		if colors[i] == value:
			x.append(reducedX[i][0])
			y.append(reducedX[i][1])
	area = [np.pi * (5**2) for i in range(len(x))]
	scatter = plt.scatter(x, y, s=area, c=[value for i in range(len(x))], alpha=0.5)
	scatterPlotsPerPopulation[j] = scatter
	legendsPerPopulation[j] = key
	j = j + 1

plt.legend(tuple(scatterPlotsPerPopulation),
           tuple(legendsPerPopulation),
           scatterpoints=1,
           loc='lower left',
           ncol=3,
           fontsize=8)

plt.show()




