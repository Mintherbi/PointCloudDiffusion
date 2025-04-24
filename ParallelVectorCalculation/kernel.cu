
#include "cuda_runtime.h"
#include "device_launch_parameters.h"

#include <stdio.h>
#include <stdlib.h>
#include <string.h>

__global__ void SingleBlockAdd(double* _pt1, double* _pt2, double* _r) {
    int tID = threadIdx.x;
    _r[tID] = _pt1[tID] + _pt2[tID];
}

__global__ void MultipleBlockAdd(double* _pt1, double* _pt2, double* _r) {
    int tID = blockIdx.x * blockDim.x + threadIdx.x;
    _r[tID] = _pt1[tID] + _pt2[tID];
}

#define EXPORTED_METHOD extern "C" __declspec(dllexport) 

EXPORTED_METHOD
void VectorAdd(double* point1, double* point2, int len, double* result)
{
    double* dpt1;
    double* dpt2; 
    double* dresult;

    int memSize = sizeof(double) * len * 3;

    cudaMalloc(&dpt1, memSize); cudaMemset(dpt1, 0, memSize);
    cudaMalloc(&dpt2, memSize); cudaMemset(dpt2, 0, memSize);
    cudaMalloc(&dresult, memSize); cudaMemset(dresult, 0, memSize);

    cudaMemcpy(dpt1, point1, memSize, cudaMemcpyHostToDevice);
    cudaMemcpy(dpt2, point2, memSize, cudaMemcpyHostToDevice);

    SingleBlockAdd <<<1, len * 3 >>> (dpt1, dpt2, dresult);
    
    cudaMemcpy(result, dresult, memSize, cudaMemcpyDeviceToHost);

    cudaFree(dpt1);
    cudaFree(dpt2);
    cudaFree(dresult);    
}

EXPORTED_METHOD
void BlockVectorAdd(double* point1, double* point2, int len, double* result)
{
    double* dpt1;
    double* dpt2; 
    double* dresult;

    int memSize = sizeof(double) * len * 3;

    cudaMalloc(&dpt1, memSize); cudaMemset(dpt1, 0, memSize);
    cudaMalloc(&dpt2, memSize); cudaMemset(dpt2, 0, memSize);
    cudaMalloc(&dresult, memSize); cudaMemset(dresult, 0, memSize);

    cudaMemcpy(dpt1, point1, memSize, cudaMemcpyHostToDevice);
    cudaMemcpy(dpt2, point2, memSize, cudaMemcpyHostToDevice);

    MultipleBlockAdd <<<ceil((len*3)/1024), 1024>>> (dpt1, dpt2, dresult);
    
    cudaMemcpy(result, dresult, memSize, cudaMemcpyDeviceToHost);

    cudaFree(dpt1);
    cudaFree(dpt2);
    cudaFree(dresult);  

}

