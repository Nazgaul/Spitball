<template>
    <img :src="srcImage" />
</template>

<script>
export default {
  props: {
    src: {},
    element: {}
  },
  data: () => ({
     observer: null, 
     intersected: false,
     intersectNotSuppoerted: false, 

  }),
  computed: {
    srcImage() {
      if(this.intersectNotSuppoerted){
        return this.src;
      }else{
        return this.intersected ? this.src : '';
      }
    }
  },
  mounted() {
    if ('IntersectionObserver' in window) {
        this.observer = new IntersectionObserver((entries) => {
          const image = entries[0];
          if (image.isIntersecting) {
              this.intersected = true;
              this.observer.disconnect();
          }
        });
        this.observer.observe(this.element);
    }else{
      this.intersectNotSuppoerted = true;
    }
  },
  destroyed() {
    this.observer.disconnect();
  }
}
</script>