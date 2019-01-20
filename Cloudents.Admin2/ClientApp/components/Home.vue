<template>
    <div data-app>
      <!--<h1>Welcome to the ADMIN portal</h1>-->
        <user-main-view></user-main-view>
    </div>
</template>

<script>
    import userMainView from '../components/userMainView/userMainView.vue'
export default {
        components:{userMainView}
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
h3 {
  margin: 40px 0 0;
}
ul {
  list-style-type: none;
  padding: 0;
}
li {
  display: inline-block;
  margin: 0 10px;
}
a {
  color: #42b983;
}
</style>
