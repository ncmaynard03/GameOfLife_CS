using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace GameOfLife
{
    public class Shader
    {
        public int Handle { get; private set; }

        public Shader(string vertexPath, string fragmentPath)
        {
            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(vertexShader, System.IO.File.ReadAllText(vertexPath));
            GL.CompileShader(vertexShader);
            CheckShaderCompile(vertexShader);

            GL.ShaderSource(fragmentShader, System.IO.File.ReadAllText(fragmentPath));
            GL.CompileShader(fragmentShader);
            CheckShaderCompile(fragmentShader);

            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, vertexShader);
            GL.AttachShader(Handle, fragmentShader);
            GL.LinkProgram(Handle);
            CheckProgramLink(Handle);

            GL.DetachShader(Handle, vertexShader);
            GL.DetachShader(Handle, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        private void CheckShaderCompile(int shader)
        {
            GL.GetShader(shader, ShaderParameter.CompileStatus, out var status);
            if (status == (int)All.False)
            {
                var infoLog = GL.GetShaderInfoLog(shader);
                throw new Exception($"Shader compilation failed: {infoLog}");
            }
        }

        private void CheckProgramLink(int program)
        {
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var status);
            if (status == (int)All.False)
            {
                var infoLog = GL.GetProgramInfoLog(program);
                throw new Exception($"Program linking failed: {infoLog}");
            }
        }

        public void Bind()
        {
            GL.UseProgram(Handle);
        }

        public void SetMatrix4(string name, Matrix4 matrix)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.UniformMatrix4(location, false, ref matrix);
        }

        public void SetColor(Vector4 color)
        {
            int location = GL.GetUniformLocation(Handle, "color");
            GL.Uniform4(location, color);
        }

        public void Dispose()
        {
            GL.DeleteProgram(Handle);
        }
    }


}