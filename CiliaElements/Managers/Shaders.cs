namespace CiliaElements
{
    public static partial class TManager
    {
        //public static TLink MainAsteroid;

        #region Private Fields

        private static string @SourceFragmentShader =
            "#version 330\n" +
            "const vec3 lightVecNormalizedXP = vec3(0,0,1);" +
            "const vec4 light = vec4 (0.1,0.1,0.1,1);" +
            "in vec3 normal;" +
            "in vec2 text;" +
            "uniform bool no_effect;" +
            "uniform mat4 color_matrix;" +
            "uniform sampler2D textureMap;" +
            "out vec4 out_frag_color;" +
            "void main(void)" +
            "{" +
            "  vec4 color = texture2D( textureMap , text );" +
            "  if ( no_effect )" +
            "  {" +
            "    out_frag_color = texture2D( textureMap , vec2(text.x + 0.5, text.y) ) * ( color + light);" +
            "  }" +
            "  else" +
            "  {" +
            "    float diffuse =clamp(dot(lightVecNormalizedXP, normal),-1.0, 1.0);" +
            "    if (diffuse < 0) diffuse=-diffuse;" +
            "    out_frag_color = texture2D( textureMap , vec2(text.x + 0.5, text.y) ) * ( diffuse + 0.2) * ( color + light)+ vec4( (color_matrix * ( diffuse + 0.2) * ( color + light)).xyz ,color.w);" +
            "  }" +
            "}";



        //private static string @SourceFragmentShader2 =
        //                    "#version 330\n" +
        //    "const vec3 lightVecNormalizedXP = vec3(0,0,1);" +
        //    "const vec4 light = vec4 (0.1,0.1,0.1,1);" +
        //    "in vec3 normal;" +
        //    "in vec2 text;" +
        //    "uniform mat4 color_matrix;" +
        //    "uniform sampler2D textureMap;" +
        //    "out vec4 out_frag_color;" +
        //    "void main(void)" +
        //    "{" +
        //    "  float diffuse =clamp(dot(lightVecNormalizedXP, normal), 0.0, 1.0);" +
        //    "  vec4 color = texture2D( textureMap , text );" +
        //    "  out_frag_color = texture2D( textureMap , vec2(text.x + 0.5, text.y) ) * ( diffuse + 0.2) * ( color + light)+ vec4( (color_matrix * ( diffuse + 0.2) * ( color + light)).xyz ,color.w);" +
        //    "}";

        private static string @SourceVertexShader =
           "#version 330\n" +
           "layout (location = 0) in vec3 in_position;" +
           "layout (location = 1) in vec3 in_normal;" +
           "layout (location = 2) in vec2 in_text;" +
           "const vec3 no_diffuse_normal = vec3(0,0,1);" +
           "uniform mat4 projection_matrix;" +
           "uniform mat4 model_matrix;" +
           "uniform mat4 view_matrix;" +
           "uniform bool no_diffuse;" +
           "uniform bool texture_offset;" +
           "uniform sampler2D textureMap;" +
           "out vec3 normal;" +
           "out vec2 text;" +
           "void main()" +
           "{" +
           "   text = in_text;" +
           "   if (no_diffuse ) " +
           "       normal = no_diffuse_normal;" +
           "   else" +
           "       normal = normalize((  model_matrix * vec4(in_normal,0)).xyz);" +
           "   gl_Position = projection_matrix *   model_matrix * vec4(in_position,1.0);" +
           "}";

        #endregion Private Fields
    }
}