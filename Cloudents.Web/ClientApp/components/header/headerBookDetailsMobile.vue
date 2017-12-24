<template>
    <div>
        <first-line :name="name"></first-line>
       <type-filter-tab :id="id"></type-filter-tab>
    </div>
</template>

<script>
    import TypeFilterTab from "../details/bookDetailsTabsMobile.vue"
    import FirstLine from './headerFirstLineMobile.vue'
    export default {
        name: "header-book-details-mobile",
        components:{FirstLine,TypeFilterTab},
        props:{name:{type:String},id:{}}
    }
</script>

<style scoped>

</style>