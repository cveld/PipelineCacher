# PipelineCacher
Running only the pipeline stages that make sense

I am currently working on this side project that ultimately will enable Azure Pipelines users to run / rerun the minimum amount of pipeline stages to achieve the desired state.


# Problem description
For various workloads I am confronted with many Azure Pipelines pipelines that we create to accomodate the desired state.

After having created a couple of pipelines, having run them with some sample inputs, quickly it becomes unclear which pipelines have to run or rerun to change the actual state into the desired state.

# Solution

The idea is that this web app "Pipeline Cacher" is able to keep track of actual state and compare the current version of run yaml files and dependencies against it.

Only the stages for which the definition and/or the dependencies (parameters, variables, files, external resources) have changed will be run.


# Design
We define a couple of layers of abstraction:
1. Pipeline Automation. One per workload.
2. Pipeline Automation Context. One per set of input parameters.
3. Pipeline. The actual yaml file.
4. Pipeline Context. A pipeline can have many contexts; one per set of input parameters. Mapping from Pipeline Automation Context to Pipeline Context.


# Backlog
We will first set up a Blazor app with database so that a user can pull pipeline information into "Pipeline Cacher".

# Authentication
For now we will be using a personal access token (PAT) to fetch data from Azure DevOps REST APIs. A "Pipeline Cacher" user can have access to many Azure DevOps contexts. Every context has its own PAT.
