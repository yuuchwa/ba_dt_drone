# MARS geo-referenced Model Starter

This project shows how to integrate your own georeferenced simulation region into a MARS model. The georeferenced data include graphs from [OpenStreetMap (OSM)](https://www.openstreetmap.org/) and Point of Interest (POI) data from [Geofabrik](https://www.geofabrik.de/de/index.html)/OSM. The functionality for obtaining georeferenced data is encapsulated in a provided Docker container.

## Running the Docker Container

There are several ways to run and use the Docker container, four of which are outlined below.

### Option 1: Local setup in JupyterLab

To set up the container on your own machine, please follow these steps:

1. [Install Docker Desktop](https://www.docker.com/get-started/) and start it.
2. Run the bash script `./notebookdocker.sh` on Linux/macOS or the `./notebookdocker.bat` on Windows in your terminal. Do not close the terminal after the script has finished running (doing so would shut down the container).
3. Open JupyterLab on [http://localhost:8888/](http://localhost:8888/).

### Option 2: Remote setup in the HAW ICC

Alternatively, if you have no access to Docker (e.g., when working on a HAW lab computer), you can use the [JupyterHub of the HAW ICC](https://jupyterhub.informatik.haw-hamburg.de/hub/login). Note that a HAW account is needed for this.

1. Login into the JupyterHub.
2. Select an instance (no GPUs are needed).
3. Upload the three `*.ipynb` files and open and execute them (see steps below).

The documentation of the HAW JupyterHub can be [found here](https://icc.informatik.haw-hamburg.de/docs/services/jupyterhub/).

### Option 3: Remote setup on Google Colab

Another alternative is to use [Google Colab](https://colab.research.google.com/). Visit Google Colab, start a session, and upload the three `*.ipynb` files and open and execute them (see steps below).

Note that you usually have no persistent storage on Google Colab. Any downloaded data need to be saved locally before the Google Colab session is terminated.

### Option 4: Using your own Jupyter Notebook instance

With the provided Docker container, you have a ready-to-use JupyterLab instance with all installed dependencies. However, you are free to use your own Jupyter Notebook instance instead of using the provided Docker container. In this case, you will need to install the required dependencies yourself.

**NOTE:** Some of the Geographic Information System (GIS) dependencies of the used Python libraries are known to be difficult to install. Please see [this](https://stackoverflow.com/questions/54734667/error-installing-geopandas-a-gdal-api-version-must-be-specified-in-anaconda) and [this](https://stackoverflow.com/questions/62299567/error-installing-geopandas-a-gdal-api-version-must-be-specified-in-visual-st) StackOverflow post for more information. If you work on a Windows machine and are unable to install dependencies with `pip`, please see this [Wheel (WHL) file depository](https://www.lfd.uci.edu/~gohlke/pythonlibs/).

## How to Use the Notebooks and Model

Once you have set up the Docker container (or set up your own Jupyter Notebook instanced properly), you can start to work with this blueprint. The blueprint provides two key functionalities:

- Downloading georeferenced vector data
- Integrating these data into a MARS model blueprint

Georeferenced data can be downloaded with the provided Jupyter notebooks, and agent-based simulations (that use the downloaded georeferenced data) can be run with the provided MARS model blueprint. The notebooks provide a starting point for downloading data and further filtering/adjusting them to your needs. Specifically, the notebooks provide the following functionality:

- `Download Graph.ipynb`: downloads georeferenced graph data (e.g., travel networks)
- `Prepare POIs.ipynb`: downloads georeferenced point-of-interest data (e.g., buildings, parking areas, etc.)
- `Analyze.ipynb`: provides analysis of simulation results

A typical workflow for using this blueprint might consist of the following steps:

1. Define an area of interest (AOI)
2. Obtain travel layers for your AOI using `Download Graph.ipynb`
3. Obtain POI data for your AOI using `Prepare POIs.ipynb`
4. Run simulations of the provided MARS model (with the data obtained in steps 2 and 3)
5. Analyze simulation results using `Analyze.ipynb`

Defining an AOI (step 1) is up to you and usually depends on your research question and/or modelling needs. Steps 2&mdash;5 are outlined in the following four sections, respectively.

### Step 1: Defining an AOI

The AOI in which the simulation will be situated needs to be defined and described in a Well-Known Text (WKT) geometry file. Such a file can be generated with, for example, [https://geojson.io/](https://geojson.io/). Follow these steps to draw and save a rectangle that represents your AOI:

1. From the toolbar at the top right of the map editor, select the button with the square
2. Double-click on the map where you want to start drawing the rectangle
3. Drag your mouse to shape the rectangle
4. Click to fix the rectangle
5. Save the rectangle as a WKT file (`Save > WKT` in the menu above the map).

### Step 2: Getting graph data

For movement of our agents, we need a street network (graph). To get such a graph, we use data from OSM.

After you saved the file, open the provided Jupyter Notebook `Download Graph.ipynb` in JupyterHub for further information. Make sure you see the downloaded WKT file alongside the notebook JupyterHub, if not upload it.

### Step 3: Getting POI data

For POI data, we utilize OSM data as well. The people of Geofabrik have done some preprocessing of raw OSM data, which makes the ingestion into the model easier. See the .shp file download for South Africa here: [south-africa-latest-free.shp.zip](http://download.geofabrik.de/africa/south-africa.html) (all other countries are provided as well). For our AOI of Port Elizabeth, we now need to extract all relevant POIs like restaurants, cafes, shops, etc.

For this, please download the data and have a look at the `Prepare POIs.ipynb` notebook, as before you might need to upload and rename the WKT file as well.

### Step 4: Running the model

In this starter template, the data needed for running a simulation on the area of Port Elizabeth have already been downloaded and put into the Resources folder of the model at `GeodataBlueprint/Resources`.

If you did run the both notebooks with a new AOI and the provided Docker container, the files have already been updated (to prevent data loos existing files for the graph and POIs are renamed to `bkp_*` files in the Resources directory if you need them later).  
In case you did run the notebooks on an other JupyterHub download the created `GeodataBlueprint/Resources` folder and copy it's contents into the local folder.

To run the model open the `GeodataBlueprint.sln` with Rider and run it.

After running the model, some log files and movement files are available that we now can analyze.

### Step 5: Analysis

For analysis of the created data, please have a look at the `Analyze.ipynb` notebook. It shows some basic statistics about the model as well as an interactive map with the movement of your agents.
