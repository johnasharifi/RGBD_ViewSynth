using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayUtil
{
	public static T[,] ArrayOfValue<T>(T defaultValue, int rows, int cols) {
		T[,] grid = new T[rows, cols];

		for (int i = 0; i < rows; ++i) {
			for (int j = 0; j < cols; ++j) {
				grid[i, j] = defaultValue;
			}
		}

		return grid;
	}
}
