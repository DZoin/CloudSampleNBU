var mapDrafting;
var allVertexGraphics;
var verticesAdded = false;

function DrawMap(mapJson) {

    if (mapJson == null) {

        alert('There is no map for this analysis.');
        return;
    }

    mapDrafting = JSON.parse(mapJson);
    if (mapDrafting == null) {

        alert('There is no map for this analysis.');
        return;
    }

    if (mapDrafting.Circles == null && mapDrafting.Polylines == null) {

        alert('The map for this analysis is empty.');
        return;
    }

    $('#divMap').show();

    window.require([
      "esri/map",
      "esri/geometry/Point",
      "esri/geometry/Circle",
      "esri/symbols/SimpleFillSymbol",
      "esri/symbols/SimpleLineSymbol",
      "esri/symbols/TextSymbol",
      "esri/symbols/Font",
      "esri/geometry/Polyline",
      "esri/units",
      "esri/Color",
      "esri/SpatialReference",
      "esri/symbols/SimpleMarkerSymbol",
      "esri/graphic",
      "esri/graphicsUtils"], function (
          Map,
          Point,
          Circle,
          SimpleFillSymbol,
          SimpleLineSymbol,
          TextSymbol,
          Font,
          Polyline,
          Units,
          Color,
          SpatialReference,
          SimpleMarkerSymbol,
          Graphic,
          GraphicUtils) {

          var map = new Map("divMap", {
              basemap: "topo"
          });

          map.on("load", function () {

              if (mapDrafting == null) {

                  return;
              }

              var allGraphics = new Array();

              // Initialize pens
              var wgs1984 = new SpatialReference(4326);
              var font = new Font("12px", Font.STYLE_NORMAL, Font.VARIANT_NORMAL, Font.WEIGHT_BOLD);

              var colorSolidBlack = new Color([0, 0, 0]);
              var colorHalfTransparentBlack = Color([0, 0, 0, 0.5]);

              var locationSymbolMarker = new SimpleMarkerSymbol(
                    SimpleMarkerSymbol.STYLE_CIRCLE,
                    10,
                    new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([255, 0, 0]), 1),
                    colorHalfTransparentBlack);

              var filledLocationCircleSymbol = new SimpleFillSymbol(
                          SimpleFillSymbol.STYLE_SOLID,
                          new SimpleLineSymbol(
                              SimpleLineSymbol.SOLID,
                              colorSolidBlack,
                              1),
                          new Color([255, 153, 51, 0.3]));
              var emptyLocationCircleSymbol = new SimpleFillSymbol(
                          SimpleFillSymbol.STYLE_SOLID,
                          new SimpleLineSymbol(
                              SimpleLineSymbol.SOLID,
                              colorSolidBlack,
                              1),
                          null);

              var outerPolylineSymbol = new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([255, 153, 51, 0.70]), 6);
              var innerPolylineSymbol = new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([0, 0, 0]), 1);
              var vertexSymbol = new SimpleMarkerSymbol(
                    SimpleMarkerSymbol.STYLE_CIRCLE,
                    4,
                    innerPolylineSymbol,
                    colorSolidBlack);

              // Draft the map
              // 1. Circles
              if (mapDrafting.Circles != null) {

                  for (var i = 0; i < mapDrafting.Circles.length; i++) {

                      var draftedCircle = mapDrafting.Circles[i];

                      var point = new Point(draftedCircle.Longitude, draftedCircle.Latitude, wgs1984);
                      var pointGraphic = new Graphic(point, locationSymbolMarker);
                      map.graphics.add(pointGraphic);
                      allGraphics.push(pointGraphic);

                      var circle = new Circle(point, {
                          radius: draftedCircle.Radius,
                          radiusUnit: Units.METERS,
                          geodesic: true
                      });

                      var circleFillSymbol = emptyLocationCircleSymbol;
                      if (draftedCircle.IsFilled) {

                          circleFillSymbol = filledLocationCircleSymbol;
                      }

                      var circleGraphic = new Graphic(circle, circleFillSymbol);
                      map.graphics.add(circleGraphic);
                      allGraphics.push(circleGraphic);

                      var textSymbol = new TextSymbol(draftedCircle.Label, font, colorSolidBlack).setOffset(0, 8);

                      var labelPointGraphic = new Graphic(point, textSymbol);
                      map.graphics.add(labelPointGraphic);
                      allGraphics.push(labelPointGraphic);
                  }
              }
              // 2. Polylines
              if (mapDrafting.Polylines != null) {

                  for (var j = 0; j < mapDrafting.Polylines.length; j++) {

                      var draftedPolyline = mapDrafting.Polylines[j];
                      var polylineVertices = new Array();
                      allVertexGraphics = new Array();

                      if (draftedPolyline.Coordinates == null) {
                          
                          continue;
                      }

                      for (var k = 0; k < draftedPolyline.Coordinates.length; k++) {

                          var pointCoordinates = draftedPolyline.Coordinates[k];
                          polylineVertices.push([pointCoordinates.m_Item2, pointCoordinates.m_Item1]);

                          var vertexPoint = new Point(pointCoordinates.m_Item2, pointCoordinates.m_Item1, wgs1984);
                          var vertexGraphic = new Graphic(vertexPoint, vertexSymbol);
                          allVertexGraphics.push(vertexGraphic);
                      }

                      var singlePathPolyline = new Polyline(polylineVertices).setSpatialReference(wgs1984);

                      var outerPolylineGraphic = new Graphic(singlePathPolyline, outerPolylineSymbol);
                      map.graphics.add(outerPolylineGraphic);
                      allGraphics.push(outerPolylineGraphic);

                      var innerPolylineGraphic = new Graphic(singlePathPolyline, innerPolylineSymbol);
                      map.graphics.add(innerPolylineGraphic);
                      allGraphics.push(innerPolylineGraphic);
                  }
              }

              var mapExtent = GraphicUtils.graphicsExtent(allGraphics);
              if (mapExtent) {

                  map.setExtent(mapExtent);
              }
          });

          map.on("zoom-end", function (e) {

              if (map == null) {

                  return;
              }

              if (allVertexGraphics == null || allVertexGraphics.length == 0) {

                  return;
              }

              var vertexGraphic;

              if (e.level < 0) {

                  return;
              }

              if (e.level <= 14) {

                  if (verticesAdded) {
                      for (var i = 0; i < allVertexGraphics.length; i++) {

                          vertexGraphic = allVertexGraphics[i];
                          map.graphics.remove(vertexGraphic);
                      }

                      verticesAdded = false;
                  }
              } else {

                  if (!verticesAdded) {
                      for (var j = 0; j < allVertexGraphics.length; j++) {

                          vertexGraphic = allVertexGraphics[j];
                          map.graphics.add(vertexGraphic);
                      }

                      verticesAdded = true;
                  }
              }
          });
      });
};

function ShowErrorMessage(errorMessage) {

    alert(errorMessage);
}