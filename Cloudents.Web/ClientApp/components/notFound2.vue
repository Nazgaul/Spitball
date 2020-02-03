<template>
    <div></div>
</template>
<script>
export default {
     created: function() {
        // Redirect outside the app using plain old javascript
        window.location.href = "/error/notfound?client=true";
    }
}
</script>