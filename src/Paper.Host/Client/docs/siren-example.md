{
   "class":[
      "orders"
   ],
   "properties":{
      "component":"grid",
      "headers": [
        {
          "text": "Dessert (100g serving)",
          "align": "left",
          "sortable": false,
          "value": "name"
        },
        { "text": "Calories", "value": "calories" },
        { "text": "Fat (g)", "value": "fat" },
        { "text": "Carbs (g)", "value": "carbs" },
        { "text": "Protein (g)", "value": "protein" },
        { "text": "Sodium (mg)", "value": "sodium" },
        { "text": "Calcium (%)", "value": "calcium" },
        { "text": "Iron (%)", "value": "iron" }
      ]
   },
   "entities":[
     {
       "class": "[order]",
       "rel": [ "http://x.io/rels/customer" ],
       "properties": {
         "value": false,
         "name": "Frozen Yogurt",
         "calories": 159,
         "fat": 6.0,
         "carbs": 24,
         "protein": 4.0,
         "sodium": 87,
         "calcium": "14%",
         "iron": "1%"
       }
     },
     {
       "class": "[order]",
       "rel": [ "http://x.io/rels/customer" ],
       "properties": {
         "value": false,
         "name": "Ice cream sandwich",
         "calories": 237,
         "fat": 9.0,
         "carbs": 37,
         "protein": 4.3,
         "sodium": 129,
         "calcium": "8%",
         "iron": "1%"
        }
     },
     {
       "class": "[order]",
       "rel": [ "http://x.io/rels/customer" ],
       "properties": {
         "value": false,
         "name": "Eclair",
         "calories": 262,
         "fat": 16.0,
         "carbs": 23,
         "protein": 6.0,
         "sodium": 337,
         "calcium": "6%",
         "iron": "7%"
       }
     },
     {
       "class": "[order]",
       "rel": [ "http://x.io/rels/customer" ],
       "properties": {
         "value": false,
         "name": "Cupcake",
         "calories": 305,
         "fat": 3.7,
         "carbs": 67,
         "protein": 4.3,
         "sodium": 413,
         "calcium": "3%",
         "iron": "8%"
       }
     },
     {
       "class": "[order]",
       "rel": [ "http://x.io/rels/customer" ],
       "properties": {
         "value": false,
         "name": "Gingerbread",
         "calories": 356,
         "fat": 16.0,
         "carbs": 49,
         "protein": 3.9,
         "sodium": 327,
         "calcium": "7%",
         "iron": "16%"
       }
     },
     {
       "class": "[order]",
       "rel": [ "http://x.io/rels/customer" ],
       "properties": {
         "value": false,
         "name": "Jelly bean",
         "calories": 375,
         "fat": 0.0,
         "carbs": 94,
         "protein": 0.0,
         "sodium": 50,
         "calcium": "0%",
         "iron": "0%"
       }
     },
     {
       "class": "[order]",
       "rel": [ "http://x.io/rels/customer" ],
       "properties": {
         "value": false,
         "name": "Lollipop",
         "calories": 392,
         "fat": 0.2,
         "carbs": 98,
         "protein": 0,
         "sodium": 38,
         "calcium": "0%",
         "iron": "2%"
       }
     },
     {
       "class": "[order]",
       "rel": [ "http://x.io/rels/customer" ],
       "properties": {
         "value": false,
         "name": "Honeycomb",
         "calories": 408,
         "fat": 3.2,
         "carbs": 87,
         "protein": 6.5,
         "sodium": 562,
         "calcium": "0%",
         "iron": "45%"
       }
     },
     {
       "class": "[order]",
       "rel": [ "http://x.io/rels/customer" ],
       "properties": {
         "value": false,
         "name": "Donut",
         "calories": 452,
         "fat": 25.0,
         "carbs": 51,
         "protein": 4.9,
         "sodium": 326,
         "calcium": "2%",
         "iron": "22%"
       }
     },
     {
       "class": "[order]",
       "rel": [ "http://x.io/rels/customer" ],
       "properties": {
         "value": false,
         "name": "KitKat",
         "calories": 518,
         "fat": 26.0,
         "carbs": 65,
         "protein": 7,
         "sodium": 54,
         "calcium": "12%",
         "iron": "6%"
       }
     }
   ],
   "actions":[
      {
         "name":"add-item",
         "title":"Add Item",
         "method":"POST",
         "href":"http://api.x.io/orders/42/items",
         "type":"application/x-www-form-urlencoded",
         "fields":[
            {
               "name":"orderNumber",
               "type":"hidden",
               "value":"42"
            },
            {
               "name":"productCode",
               "type":"text"
            },
            {
               "name":"quantity",
               "type":"number"
            }
         ]
      }
   ],
   "links":[
      {
         "rel":[
            "self"
         ],
         "href":"http://api.x.io/orders/42"
      },
      {
         "rel":[
            "previous"
         ],
         "href":"http://api.x.io/orders/41"
      },
      {
         "rel":[
            "next"
         ],
         "href":"http://api.x.io/orders/43"
      }
   ]
}