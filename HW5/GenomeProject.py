import numpy as np
import matplotlib.pyplot as plt
import plotly.plotly as py
from collections import Counter
from math import sqrt
from sklearn.decomposition import PCA
from sklearn.preprocessing import StandardScaler
from array import array

# Initialize values
individuals = 995
columns = 10101
fullData = [[0 for j in range(columns + 3)] for i in range(individuals)] 
X = [[0 for j in range(columns)] for i in range(individuals)]

plt.figure(figsize=(16, 8), dpi=80)

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

# Section (b)
# pca = PCA(n_components=2)
# X_std = StandardScaler().fit_transform(X)
# reducedX = pca.fit_transform(X_std)

# Section (d)
def normalized(v):
	return v/np.sqrt(np.dot(v, v))

def ortho_proj_vec(v, uhat):
	return v-np.dot(v,uhat)*uhat

pca = PCA(n_components=3)
X_std = StandardScaler().fit_transform(X)
pca.fit(X_std)

v1hat=normalized(pca.components_[0])
v2hat=normalized(ortho_proj_vec(pca.components_[2], v1hat))

reducedX=[0 for i in range(individuals)]
for idx in xrange(X_std.shape[0]):
	reducedX[idx]=ortho_proj_vec(ortho_proj_vec(X_std[idx,:], v1hat), v2hat)

# Section (f)
for i in range(len(pca.components_[2])):
	# if abs(pca.components_[2][i]) > 0.1:
	print("{} : {}".format(i, pca.components_[2][i]))

# Draw scatter by population
# populationMap = {
# 	"ACB" : "African Caribbeans in Barbados",
# 	"ASW" : "Americans of African Ancestry in SW USA",
# 	"ESN" : "Esan in Nigeria",
# 	"GWD" : "Gambian in Western Divisions in the Gambia",
# 	"LWK" : "Luhya in Webuye, Kenya",
# 	"MSL" : "Mende in Sierra Leone",
# 	"YRI" : "Yoruba in Ibadan, Nigeria"
# }
# cmap = plt.cm.get_cmap("hsv", len(populationMap) + 1)

# scatterPlotsPerPopulation = [0 for i in range(len(populationMap))]
# legendsPerPopulation = [0 for i in range(len(populationMap))]
# j = 0
# for key, value in populationMap.iteritems():
# 	x = []
# 	y = []
# 	for i in range(individuals):
# 		if fullData[i][2] == key:
# 			x.append(reducedX[i][0])
# 			y.append(reducedX[i][1])
# 	area = [np.pi * (5**2) for i in range(len(x))]
# 	scatter = plt.scatter(x, y, s=area, c=[cmap(j) for i in range(len(x))], alpha=0.5)
# 	scatterPlotsPerPopulation[j] = scatter
# 	legendsPerPopulation[j] = populationMap[key]
# 	j = j + 1

# plt.legend(tuple(scatterPlotsPerPopulation),
#            tuple(legendsPerPopulation),
#            scatterpoints=1,
#            loc='lower left',
#            ncol=3,
#            fontsize=8)

# Draw scatter by gender
# genderMap = {
# 	"1" : "Male",
# 	"2" : "Female"
# }
# cmap = plt.cm.get_cmap("hsv", len(genderMap) + 1)

# scatterPlotsPerGender = [0 for i in range(len(genderMap))]
# legendsPerGender = [0 for i in range(len(genderMap))]
# j = 0
# for key, value in genderMap.iteritems():
# 	x = []
# 	y = []
# 	for i in range(individuals):
# 		if fullData[i][1] == key:
# 			x.append(reducedX[i][0])
# 			y.append(reducedX[i][1])
# 	area = [np.pi * (5**2) for i in range(len(x))]
# 	scatter = plt.scatter(x, y, s=area, c=[cmap(j) for i in range(len(x))], alpha=0.5)
# 	scatterPlotsPerGender[j] = scatter
# 	legendsPerGender[j] = genderMap[key]
# 	j = j + 1

# plt.legend(tuple(scatterPlotsPerGender),
#            tuple(legendsPerGender),
#            scatterpoints=1,
#            loc='lower left',
#            ncol=3,
#            fontsize=8)


# plt.show()




