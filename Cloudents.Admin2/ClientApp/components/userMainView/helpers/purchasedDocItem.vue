<template>
    <div>
    <v-card class="elevation-5">
         <v-card-text>
              <div><b>Document Name:</b> {{document.title}}</div>
        
        
            <div><b>Document Id:</b> {{document.id}}</div>
            <div><b>University:</b> {{document.university}}</div>
            <div><b>Course:</b> {{document.class}}</div>
            <div><b>Price:</b>  {{document.price}}</div>
        </v-card-text>
    </v-card>
    </div>
</template>

<script>
    export default {
        name: "documentItem",
         data() {
            return {
               
            }
        },
      
        props: {
            document: {},
            filterVal: {
                type: String,
                required: false
            },
        }
    }
</script>

<style lang="less">
   
</style>