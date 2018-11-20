<template>
  <div id="template-app">
        <nav id="nav">
            <router-link to="/home">Home</router-link> 
            <router-link to="/question">Question</router-link>
            <router-link to="/user">User</router-link>
            <router-link to="/dev">Dev Only</router-link>
        </nav>
        <div id="app-body">
            <router-view/>
        </div>
    </div>
</template>

<script>
export default {
}
</script>

<style lang="scss">
   html,body {
        height: 100%;
        margin: 0px;
        padding: 0px; 
        background-color: #e0e0e0;
    }
    #template-app {
        font-family: 'Roboto', Helvetica, Arial, sans-serif;
        -webkit-font-smoothing: antialiased;
        -moz-osx-font-smoothing: grayscale;
        text-align: center;
        /*color: #5d5d5d;*/
        color: rgba(0,0,0,0.87);
        display: flex;
        height:100%;
    }

    #nav {
        padding: 2px;
        flex-grow: 1;
        border-right: 2px solid #7c7979;
        width: 200px;
        max-width: 200px;
        min-width: 200px;
        position: sticky;
        top: 0;
        border-radius: 25px;
    a {
        font-weight: bold;
        color: #5d5d5d;
        display:block;
        height: 45px;
        line-height: 45px;
        width:100%;
        margin-bottom: 2px;
        text-decoration: none;
        border-radius: 25px;
    &:hover{
         background-color: #7c7979;
     }
    }
    a.router-link-active {
        color: #97ed82;
        background-color: #7c7979;
    } 
    }
    #app-body{
        flex-grow: 1;
        padding: 2px;
        overflow: auto;
    }
</style>
